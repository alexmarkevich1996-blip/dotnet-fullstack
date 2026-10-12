using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Departments;

public class DepartmentLocationNotFoundException : DomainException
{
    private DepartmentLocationNotFoundException(Error error) : base(error)
    {
    }

    public static DepartmentLocationNotFoundException ForIds(Guid departmentId, Guid locationId) =>
        new(Error.NotFound(
            "department_location.not_found",
            $"Location '{locationId}' is not linked to department '{departmentId}'"));
}
