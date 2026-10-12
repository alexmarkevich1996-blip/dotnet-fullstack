using DirectoryService.Core.Departments;
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
    public async Task<Guid> AddAsync(
        Department department,
        IReadOnlyCollection<DepartmentLocation> departmentLocations,
        CancellationToken cancellationToken)
    {
        _context.Departments.Add(department);

        if (departmentLocations.Count > 0)
            _context.DepartmentLocations.AddRange(departmentLocations);

        await _context.SaveChangesAsync(cancellationToken);
        return department.Id;
    }

    public async Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(
                d => d.Id == id, cancellationToken);
    }

    public async Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        Name nameValue = Name.Create(name);
        
        return await _context.Departments
            .FirstOrDefaultAsync(
                d => d.Name == nameValue, cancellationToken);
    }

    public async Task UpdateAsync(Department department, CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddLocationAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken)
    {
        _context.DepartmentLocations.Add(departmentLocation);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        var departmentLocation = await _context.DepartmentLocations
            .FirstOrDefaultAsync(dl => dl.DepartmentId == departmentId && dl.LocationId == locationId, cancellationToken);

        if (departmentLocation is null)
            return;

        _context.DepartmentLocations.Remove(departmentLocation);

        await _context.SaveChangesAsync(cancellationToken);
    }
    public Task<bool> LocationLinkExistsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        var existingDepartmentLocation = _context.DepartmentLocations
            .FirstOrDefault(dl => dl.DepartmentId == departmentId && dl.LocationId == locationId);

        return Task.FromResult(existingDepartmentLocation is not null);
    }
}