using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Common.ValueObjects;
using Path = DirectoryService.Domain.Departments.ValueObjects.Path;

namespace DirectoryService.Domain.Departments;

public class Department
{
    public Guid Id { get; }
    public Name Name { get; private set; }
    public Slug Slug { get; private set; }
    public Path Path { get; private set; }
    public Guid? ParentId { get; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Department(Guid? parentId, Name name, Slug slug, Path path)
    {
        Id = Guid.CreateVersion7();
        Name = name;
        Slug = slug;
        Path = path;
        ParentId = parentId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Result<Department, ErrorList> Create(Department? parent, Name name, Slug slug)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(slug);

        Guid? parentId;
        Result<Path, ErrorList> pathResult;

        if (parent is null)
        {
            parentId = null;
            pathResult = Path.CreateRoot(slug);
        }
        else
        {
            parentId = parent.Id;
            pathResult = Path.CreateChild(parent.Path, slug);
        }

        if (pathResult.IsFailure)
            return pathResult.Error;

        return new Department(parentId, name, slug, pathResult.Value);
    }

    public void UpdateName(Name name)
    {
        ArgumentNullException.ThrowIfNull(name);
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}