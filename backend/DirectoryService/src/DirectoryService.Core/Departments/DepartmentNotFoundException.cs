using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Departments;

public class DepartmentNotFoundException : DomainException
{
    private DepartmentNotFoundException(Error error) : base(error)
    {
    }
    public static DepartmentNotFoundException ForId(Guid id) =>
        new(Error.NotFound("department.not_found", $"Department with id '{id}' was not found"));
}
