using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Departments;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsRepository
{
    Task<Result<Guid, ErrorList>> AddAsync(
        Department department,
        IReadOnlyCollection<DepartmentLocation> departmentLocations,
        CancellationToken cancellationToken);
    Task<Result<Department, ErrorList>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Department?> GetByNameAsync(Name name, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> UpdateAsync(Department department, CancellationToken cancellationToken);
    Task<bool> LocationLinkExistsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> AddLocationAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> RemoveLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
}