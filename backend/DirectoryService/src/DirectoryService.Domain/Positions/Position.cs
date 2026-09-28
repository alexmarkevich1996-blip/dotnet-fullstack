using DirectoryService.Domain.Common.ValueObjects;

namespace DirectoryService.Domain.Positions;

public class Position
{
    public Guid Id { get; }
    public Name Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Position(Name name)
    {
        Id = Guid.CreateVersion7();
        Name = name;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Position Create(Name name)
    {
        ArgumentNullException.ThrowIfNull(name);
        
        return new Position(name);
    }
}