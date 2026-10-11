namespace DirectoryService.Core.Departments;

public class DepartmentAlreadyExistsException : Exception
{
    public DepartmentAlreadyExistsException()
    {
    }

    public DepartmentAlreadyExistsException(string message)
        : base(message)
    {
    }

    public DepartmentAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public static DepartmentAlreadyExistsException ForName(string name) =>
        new($"Department with name '{name}' already exists");
}
