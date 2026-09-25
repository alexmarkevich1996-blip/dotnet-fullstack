namespace DirectoryService.Domain.Departments;

public class DepartmentLocation
{
    public Guid Id { get; }
    public Guid DepartmentId { get; }
    public Guid LocationId { get; }
    public bool IsPrimary { get; private set; }

    private DepartmentLocation(Guid departmentId, Guid locationId, bool isPrimary)
    {
        Id = Guid.CreateVersion7();
        DepartmentId = departmentId;
        LocationId = locationId;
        IsPrimary = isPrimary;
    }

    public static DepartmentLocation Create(Guid departmentId, Guid locationId, bool isPrimary)
    {
        if(departmentId == Guid.Empty)
            throw new ArgumentException("Department id must not be empty", nameof(departmentId));
        if(locationId == Guid.Empty)
            throw new ArgumentException("Location id must not be empty", nameof(locationId));
        
        return new DepartmentLocation(departmentId, locationId, isPrimary);
    }
}