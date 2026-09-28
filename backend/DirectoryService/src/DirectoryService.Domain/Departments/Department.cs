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

    public static Department Create(Department? parent, Name name, Slug slug)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(slug);

        Guid? parentId;
        Path path;

        if (parent is null)
        {
            parentId = null;
            path = Path.CreateRoot(slug);
        }
        else
        {
            parentId = parent.Id;
            path = Path.CreateChild(parent.Path, slug);
        }

        return new Department(parentId, name, slug, path);
    }
}