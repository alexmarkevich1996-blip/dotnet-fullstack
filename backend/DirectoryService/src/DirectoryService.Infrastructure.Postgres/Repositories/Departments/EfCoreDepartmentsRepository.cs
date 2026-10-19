using CSharpFunctionalExtensions;
using DirectoryService.Core.Departments;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Repositories.Departments;

public class EfCoreDepartmentsRepository : IDepartmentsRepository
{
    private readonly DirectoryServiceDbContext _context;

    public EfCoreDepartmentsRepository(DirectoryServiceDbContext context)
    {
        _context = context;
    }

    public Task<Result<Guid, ErrorList>> AddAsync(Department department, CancellationToken cancellationToken)
    {
        _context.Departments.Add(department);
        return Task.FromResult(Result.Success<Guid, ErrorList>(department.Id));
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

    public Task<UnitResult<ErrorList>> AddLocationAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken)
    {
        _context.DepartmentLocations.Add(departmentLocation);
        return Task.FromResult(UnitResult.Success<ErrorList>());
    }

    public async Task<UnitResult<ErrorList>> RemoveLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        var departmentLocation = await _context.DepartmentLocations
            .FirstOrDefaultAsync(dl => dl.DepartmentId == departmentId && dl.LocationId == locationId, cancellationToken);

        if (departmentLocation is null)
            return UnitResult.Success<ErrorList>();

        _context.DepartmentLocations.Remove(departmentLocation);
        return UnitResult.Success<ErrorList>();
    }

    public Task<bool> LocationLinkExistsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        var existingDepartmentLocation = _context.DepartmentLocations
            .FirstOrDefault(dl => dl.DepartmentId == departmentId && dl.LocationId == locationId);

        return Task.FromResult(existingDepartmentLocation is not null);
    }
}
