using CSharpFunctionalExtensions;
using DirectoryService.Core.Departments;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories.Departments;

public partial class EfCoreDepartmentsRepository : IDepartmentsRepository
{
    private readonly DirectoryServiceDbContext _context;
    private readonly ILogger<EfCoreDepartmentsRepository> _logger;

    public EfCoreDepartmentsRepository(DirectoryServiceDbContext context, ILogger<EfCoreDepartmentsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> AddAsync(
        Department department,
        IReadOnlyCollection<DepartmentLocation> departmentLocations,
        CancellationToken cancellationToken)
    {
        _context.Departments.Add(department);

        if (departmentLocations.Count > 0)
            _context.DepartmentLocations.AddRange(departmentLocations);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return department.Id;
        }
        catch (DbUpdateException ex)
        {
            LogSaveFailed(ex, department.Id);
            return Error.Internal("department.save_failed", "Failed to save department").ToErrors();
        }
    }

    public async Task<Result<Department, ErrorList>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (department is null)
            return Error.NotFound("department.not_found", $"Department with id '{id}' was not found").ToErrors();

        return department;
    }

    public async Task<Department?> GetByNameAsync(Name name, CancellationToken cancellationToken)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(d => d.Name == name, cancellationToken);
    }

    public async Task<UnitResult<ErrorList>> UpdateAsync(Department department, CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<ErrorList>();
        }
        catch (DbUpdateException ex)
        {
            LogSaveFailed(ex, department.Id);
            return Error.Internal("department.save_failed", "Failed to save department").ToErrors();
        }
    }

    public async Task<UnitResult<ErrorList>> AddLocationAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken)
    {
        _context.DepartmentLocations.Add(departmentLocation);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<ErrorList>();
        }
        catch (DbUpdateException ex)
        {
            LogLinkSaveFailed(ex, departmentLocation.LocationId, departmentLocation.DepartmentId);
            return Error.Internal("department_location.save_failed", "Failed to link location to department").ToErrors();
        }
    }

    public async Task<UnitResult<ErrorList>> RemoveLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        var departmentLocation = await _context.DepartmentLocations
            .FirstOrDefaultAsync(dl => dl.DepartmentId == departmentId && dl.LocationId == locationId, cancellationToken);

        if (departmentLocation is null)
            return UnitResult.Success<ErrorList>();

        _context.DepartmentLocations.Remove(departmentLocation);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<ErrorList>();
        }
        catch (DbUpdateException ex)
        {
            LogUnlinkSaveFailed(ex, locationId, departmentId);
            return Error.Internal("department_location.save_failed", "Failed to unlink location from department").ToErrors();
        }
    }

    public Task<bool> LocationLinkExistsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        var existingDepartmentLocation = _context.DepartmentLocations
            .FirstOrDefault(dl => dl.DepartmentId == departmentId && dl.LocationId == locationId);

        return Task.FromResult(existingDepartmentLocation is not null);
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to save department {DepartmentId}")]
    private partial void LogSaveFailed(Exception exception, Guid departmentId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to link location {LocationId} to department {DepartmentId}")]
    private partial void LogLinkSaveFailed(Exception exception, Guid locationId, Guid departmentId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to unlink location {LocationId} from department {DepartmentId}")]
    private partial void LogUnlinkSaveFailed(Exception exception, Guid locationId, Guid departmentId);
}
