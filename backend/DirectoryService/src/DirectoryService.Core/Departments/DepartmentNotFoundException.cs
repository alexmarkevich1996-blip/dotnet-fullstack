namespace DirectoryService.Core.Departments;

public class DepartmentNotFoundException : Exception
{
    public DepartmentNotFoundException()
    {
    }

    public DepartmentNotFoundException(string message)
        : base(message)
    {
    }

    public DepartmentNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public static DepartmentNotFoundException ForId(Guid id) =>
        new($"Department with id '{id}' was not found");
}
