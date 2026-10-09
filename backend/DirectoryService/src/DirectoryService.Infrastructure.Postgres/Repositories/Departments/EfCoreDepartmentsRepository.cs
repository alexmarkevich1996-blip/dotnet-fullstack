using DirectoryService.Core.Departments;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories.Departments;

public class EfCoreDepartmentsRepository : IDepartmentsRepository
{
    private readonly DirectoryServiceDbContext _context;
    private readonly ILogger<EfCoreDepartmentsRepository> _logger;

    public EfCoreDepartmentsRepository(
        DirectoryServiceDbContext context,
        ILogger<EfCoreDepartmentsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<Guid> AddAsync(
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
            _logger.LogDepartmentsSaveFailed(ex, department.Id);
            throw new InvalidOperationException($"Failed to save department {department.Id}.", ex);
        }
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
}