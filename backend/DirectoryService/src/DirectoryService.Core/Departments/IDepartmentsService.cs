using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsService
{
    public Task<Result<Guid, ErrorList>> Create(CreateDepartmentDto departmentDto, CancellationToken cancellationToken);
    public Task<UnitResult<ErrorList>> Update(Guid id, UpdateDepartmentDto departmentDto, CancellationToken cancellationToken);
    public Task<UnitResult<ErrorList>> AddLocation(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    public Task<UnitResult<ErrorList>> RemoveLocation(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
}