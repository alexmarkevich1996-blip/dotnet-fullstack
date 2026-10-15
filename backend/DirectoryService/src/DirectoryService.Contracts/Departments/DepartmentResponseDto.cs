namespace DirectoryService.Contracts.Departments;

public record DepartmentResponseDto(
    Guid Id,
    string Name,
    string Slug,
    string Path,
    Guid? ParentId,
    DateTime? CreatedAt,
    DateTime UpdatedAt);