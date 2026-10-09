using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Common.ValueObjects;

namespace DirectoryService.Domain.Locations;

public class Location
{
    public Guid Id { get; }
    public Name Name { get; private set; }
    public Address Address { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Location(Name name, Address address)
        : this(Guid.CreateVersion7(),  name, address, DateTime.UtcNow, DateTime.UtcNow)
    {
    }
    private Location(Guid id, Name name, Address address, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        Name = name;
        Address = address;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
    public static Location Create(Name name, Address address)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(address);
        
        return new Location(name, address);
    }

    internal static Location Rehydrate(Guid id, Name name, Address address, DateTime createdAt, DateTime updatedAt)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(address);
        if (updatedAt < createdAt)
            throw new ArgumentException(
                "UpdatedAt cannot be earlier than CreatedAt.",
                nameof(updatedAt));
        
        return new (id, name, address, createdAt, updatedAt);
    }
        
}