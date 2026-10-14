using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;

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

    public static Result<DepartmentPosition, ErrorList> Create(Guid departmentId, Guid positionId)
    {
        if (departmentId == Guid.Empty)
            return Error.Validation("department_position.department_id_empty", "Department id must not be empty").ToErrors();
        if (positionId == Guid.Empty)
            return Error.Validation("department_position.position_id_empty", "Position id must not be empty").ToErrors();

        return new DepartmentPosition(departmentId, positionId);
    }
}