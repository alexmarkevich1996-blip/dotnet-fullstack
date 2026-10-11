namespace DirectoryService.Core.Departments;

public class DepartmentLocationNotFoundException : Exception
{
    public DepartmentLocationNotFoundException()
    {
    }

    public DepartmentLocationNotFoundException(string message)
        : base(message)
    {
    }

    public DepartmentLocationNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public static DepartmentLocationNotFoundException ForIds(Guid departmentId, Guid locationId) =>
        new($"Location '{locationId}' is not linked to department '{departmentId}'");
}
