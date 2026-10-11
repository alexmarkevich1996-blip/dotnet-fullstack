namespace DirectoryService.Core.Locations;

public class LocationNotFoundException : Exception
{
    public LocationNotFoundException()
    {
    }

    public LocationNotFoundException(string message)
        : base(message)
    {
    }

    public LocationNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public static LocationNotFoundException ForIds(IReadOnlyCollection<Guid> ids) =>
        new($"Locations not found: {string.Join(", ", ids)}");
    public static LocationNotFoundException ForId(Guid id) =>
        new($"Location not found: {string.Join(", ", id)}");
    
}
