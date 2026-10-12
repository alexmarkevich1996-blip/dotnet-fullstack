using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Departments;

public class DepartmentLocationAlreadyExistsException : DomainException
{
    private DepartmentLocationAlreadyExistsException(Error error) : base(error)
    {
    }

    public static DepartmentLocationAlreadyExistsException ForIds(Guid departmentId, Guid locationId) =>
        new(Error.Conflict(
            "department_location.already_exists",
            $"Location '{locationId}' is already linked to department '{departmentId}'"));
}
