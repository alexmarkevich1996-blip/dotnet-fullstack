using DirectoryService.Contracts.Departments;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsService
{
    public Task<Guid> Create(CreateDepartmentDto departmentDto, CancellationToken cancellationToken);
}