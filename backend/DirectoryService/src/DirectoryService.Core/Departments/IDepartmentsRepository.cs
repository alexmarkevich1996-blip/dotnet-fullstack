using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Departments;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsRepository
{
    Task<Result<Guid, ErrorList>> AddAsync(Department department, CancellationToken cancellationToken);
    Task<Result<Department, ErrorList>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Department?> GetByNameAsync(Name name, CancellationToken cancellationToken);
    Task<bool> HasChildrenAsync(Guid departmentId, CancellationToken cancellationToken);
    Task<bool> HasLocationLinksAsync(Guid departmentId, CancellationToken cancellationToken);
    Task<bool> HasPositionLinksAsync(Guid departmentId, CancellationToken cancellationToken);
    void Remove(Department department);
    Task<bool> LocationLinkExistsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> AddLocationAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> RemoveLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    Task<bool> PositionLinkExistsAsync(Guid departmentId, Guid positionId, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> AddPositionAsync(DepartmentPosition departmentPosition, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> RemovePositionAsync(Guid departmentId, Guid positionId, CancellationToken cancellationToken);
}
