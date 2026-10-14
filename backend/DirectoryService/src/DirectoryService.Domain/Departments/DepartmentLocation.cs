using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;

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

    public static Result<DepartmentLocation, ErrorList> Create(Guid departmentId, Guid locationId, bool isPrimary)
    {
        if (departmentId == Guid.Empty)
            return Error.Validation("department_location.department_id_empty", "Department id must not be empty").ToErrors();
        if (locationId == Guid.Empty)
            return Error.Validation("department_location.location_id_empty", "Location id must not be empty").ToErrors();

        return new DepartmentLocation(departmentId, locationId, isPrimary);
    }
}