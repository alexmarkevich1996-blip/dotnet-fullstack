using Dapper;
using CSharpFunctionalExtensions;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DirectoryService.Infrastructure.Postgres.Repositories.Locations;

public partial class DapperLocationsRepository : ILocationsRepository
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<DapperLocationsRepository> _logger;

    public DapperLocationsRepository(NpgsqlDataSource dataSource, ILogger<DapperLocationsRepository> logger)
    {
        _dataSource = dataSource;
        _logger = logger;
    }
    public async Task<Result<Guid, ErrorList>> AddAsync(Location location, CancellationToken cancellationToken)
    {
        const string locationInsertSql = """
                                         INSERT INTO locations (id, name, address, created_at, updated_at)
                                         VALUES (@Id, @Name, @Address, @CreatedAt, @UpdatedAt)
                                         """;

        var locationsInsertParams = new
        {
            location.Id,
            Name = location.Name.Value,
            Address = location.Address.Value,
            location.CreatedAt,
            location.UpdatedAt
        };

        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await connection.ExecuteAsync(new CommandDefinition(
                locationInsertSql,
                locationsInsertParams,
                cancellationToken: cancellationToken));
            return location.Id;
        }
        catch (NpgsqlException ex)
        {
            LogSaveFailed(ex, location.Id);
            return Error.Internal("location.save_failed", "Failed to save location").ToErrors();
        }
    }

    public async Task<Location?> GetByNameAsync(Name name, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT
                                id AS Id,
                                name as Name,
                                address as Address,
                                created_at AS CreatedAt,
                                updated_at AS UpdatedAt
                           FROM locations
                           WHERE name = @Name;
                           """;
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        LocationRow? row = await connection.QuerySingleOrDefaultAsync<LocationRow>(
            new CommandDefinition(
                sql,
                new { Name = name.Value },
                cancellationToken: cancellationToken));

        if (row == null)
            return null;

        return Location.Rehydrate(
            row.Id,
            Name.FromValue(row.Name),
            Address.FromValue(row.Address),
            row.CreatedAt,
            row.UpdatedAt
        );
    }


    public async Task<Result<Location, ErrorList>> GetByIdAsync(Guid locationId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT
                                id AS Id,
                                name as Name,
                                address as Address,
                                created_at AS CreatedAt,
                                updated_at AS UpdatedAt
                           FROM locations
                           WHERE id = @Id;
                           """;
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        LocationRow? row = await connection.QuerySingleOrDefaultAsync<LocationRow>(
            new CommandDefinition(
                sql,
                new { Id = locationId },
                cancellationToken: cancellationToken));

        if (row == null)
            return Error.NotFound("location.not_found", $"Location with id '{locationId}' was not found").ToErrors();

        return Location.Rehydrate(
            row.Id,
            Name.FromValue(row.Name),
            Address.FromValue(row.Address),
            row.CreatedAt,
            row.UpdatedAt
        );
    }

    public async Task<IReadOnlyCollection<Guid>> GetExistingIdsAsync(
        IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken)
    {
        const string sql = "SELECT id FROM locations WHERE id = ANY(@Ids)";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var existingIds = await connection.QueryAsync<Guid>(
            new CommandDefinition(
                sql,
                new { Ids = ids.ToArray() },
                cancellationToken: cancellationToken));

        return existingIds.ToList();
    }

    public async Task<bool> HasDepartmentLinksAsync(Guid locationId, CancellationToken cancellationToken)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM department_locations WHERE location_id = @LocationId)";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                sql,
                new { LocationId = locationId },
                cancellationToken: cancellationToken));
    }

    public void Remove(Location location)
    {
        throw new NotSupportedException("Not implemented yet");
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to save location {LocationId}")]
    private partial void LogSaveFailed(Exception exception, Guid locationId);
}