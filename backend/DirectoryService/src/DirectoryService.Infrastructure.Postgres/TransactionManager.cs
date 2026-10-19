using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DirectoryService.Infrastructure.Postgres;

public partial class TransactionManager : ITransactionManager
{
    private readonly DirectoryServiceDbContext _context;
    private readonly ILogger<TransactionManager> _logger;

    public TransactionManager(
        DirectoryServiceDbContext context,
        ILogger<TransactionManager> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<UnitResult<ErrorList>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<ErrorList>();
        }
        catch (DbUpdateException ex)
        {
            return MapException(ex);
        }
    }

    public async Task<Result<T, ErrorList>> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<Result<T, ErrorList>>> operation,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var operationResult = await operation(cancellationToken);
            if (operationResult.IsFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
                return operationResult.Error;
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return operationResult.Value;
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return MapException(ex).Error;
        }
    }

    public async Task<UnitResult<ErrorList>> ExecuteInTransactionAsync(
        Func<CancellationToken, Task<UnitResult<ErrorList>>> operation,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var operationResult = await operation(cancellationToken);
            if (operationResult.IsFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
                return operationResult.Error;
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return UnitResult.Success<ErrorList>();
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return MapException(ex);
        }
    }

    private UnitResult<ErrorList> MapException(DbUpdateException ex)
    {
        switch (ex)
        {
            case DbUpdateConcurrencyException concurrencyEx:
                LogConcurrencyConflict(concurrencyEx);
                return Error.Conflict(
                    "database.concurrency_conflict",
                    "The record was modified by another request").ToErrors();

            case { InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } pg }:
                LogUniqueViolation(pg, pg.ConstraintName ?? "unknown");
                return MapUniqueViolation(pg.ConstraintName);

            case { InnerException: PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation } pg }:
                LogForeignKeyViolation(pg, pg.ConstraintName ?? "unknown");
                return MapForeignKeyViolation(pg.ConstraintName);

            default:
                LogDatabaseFailure(ex);
                return Error.Internal("database.save_failed", "Failed to save changes").ToErrors();
        }
    }

    private static UnitResult<ErrorList> MapUniqueViolation(string? constraintName) => constraintName switch
    {
        "ix_departments_parent_id_slug" => Error.Conflict(
            "department.slug.duplicate",
            "Department with this slug already exists under the parent").ToErrors(),
        "ix_locations_name" => Error.Conflict(
            "location.name.duplicate",
            "Location with this name already exists").ToErrors(),
        "ix_locations_address" => Error.Conflict(
            "location.address.duplicate",
            "Location with this address already exists").ToErrors(),
        "ix_department_locations_department_id_location_id" => Error.Conflict(
            "department_location.already_exists",
            "Location is already linked to this department").ToErrors(),
        _ => Error.Conflict(
            "database.unique_violation",
            "A record with this value already exists").ToErrors(),
    };

    private static UnitResult<ErrorList> MapForeignKeyViolation(string? constraintName) => constraintName switch
    {
        "fk_departments_departments_parent_id" => Error.NotFound(
            "department.parent_not_found",
            "Parent department was not found").ToErrors(),
        "fk_department_locations_departments_department_id" => Error.NotFound(
            "department.not_found",
            "Department was not found").ToErrors(),
        "fk_department_locations_locations_location_id" => Error.NotFound(
            "location.not_found",
            "Location was not found").ToErrors(),
        _ => Error.Validation(
            "database.foreign_key_violation",
            "Referenced entity does not exist").ToErrors(),
    };

    [LoggerMessage(Level = LogLevel.Warning, Message = "Concurrency conflict while saving changes")]
    private partial void LogConcurrencyConflict(Exception exception);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Unique constraint violation on {ConstraintName}")]
    private partial void LogUniqueViolation(Exception exception, string constraintName);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Foreign key violation on {ConstraintName}")]
    private partial void LogForeignKeyViolation(Exception exception, string constraintName);

    [LoggerMessage(Level = LogLevel.Error, Message = "Database save failed")]
    private partial void LogDatabaseFailure(Exception exception);
}