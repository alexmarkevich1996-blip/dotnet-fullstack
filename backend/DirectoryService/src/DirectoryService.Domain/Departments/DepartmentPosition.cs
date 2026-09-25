namespace DirectoryService.Domain.Departments;

public class DepartmentPosition
{
    public Guid Id { get; }
    public Guid DepartmentId { get; }
    public Guid PositionId { get; }
    
    private DepartmentPosition(Guid departmentId, Guid positionId)
    {
        Id = Guid.CreateVersion7();
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public static DepartmentPosition Create(Guid departmentId, Guid positionId)
    {
        if(departmentId == Guid.Empty)
            throw new ArgumentException("Department id must not be empty", nameof(departmentId));
        if(positionId == Guid.Empty)
            throw new ArgumentException("Position id must not be empty", nameof(positionId));
        
        return new DepartmentPosition(departmentId, positionId);
    }
}