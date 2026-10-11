using DirectoryService.Contracts.Departments;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsService
{
    public Task<Guid> Create(CreateDepartmentDto departmentDto, CancellationToken cancellationToken);
    public Task Update(Guid id, UpdateDepartmentDto departmentDto, CancellationToken cancellationToken);
    public Task AddLocation(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    public Task RemoveLocation(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
}