namespace DirectoryService.Core.Departments;

public class DepartmentLocationAlreadyExistsException : Exception
{
    public DepartmentLocationAlreadyExistsException()
    {
    }

    public DepartmentLocationAlreadyExistsException(string message)
        : base(message)
    {
    }

    public DepartmentLocationAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public static DepartmentLocationAlreadyExistsException ForIds(Guid departmentId, Guid locationId) =>
        new($"Location '{locationId}' is already linked to department '{departmentId}'");
}
