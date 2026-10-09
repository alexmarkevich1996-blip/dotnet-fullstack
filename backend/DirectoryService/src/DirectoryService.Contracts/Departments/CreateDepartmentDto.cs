using DirectoryService.Contracts.Locations;

namespace DirectoryService.Contracts.Departments;

public record CreateDepartmentDto(
    string Name,
    string Slug,
    Guid? ParentId,
    IReadOnlyCollection<Guid> LocationIds);
