using DirectoryService.Domain.Departments;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsRepository
{
    Task<Guid> AddAsync(
        Department department,
        IReadOnlyCollection<DepartmentLocation> departmentLocations,
        CancellationToken cancellationToken);
    Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task UpdateAsync(Department department, CancellationToken cancellationToken);
    public Task<bool> LocationLinkExistsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    public Task AddLocationAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken);
    public Task RemoveLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);


}