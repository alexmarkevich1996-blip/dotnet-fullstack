using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Departments;

public class DepartmentAlreadyExistsException : DomainException
{
    private DepartmentAlreadyExistsException(Error error) : base(error)
    {
    }

    public static DepartmentAlreadyExistsException ForName(string name) =>
        new(Error.Conflict("department.name.duplicate", $"Department with name '{name}' already exists"));
}
