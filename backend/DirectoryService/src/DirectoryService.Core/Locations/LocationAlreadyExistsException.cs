namespace DirectoryService.Core.Locations;

public class LocationAlreadyExistsException : Exception
{
    public LocationAlreadyExistsException()
    {
    }

    public LocationAlreadyExistsException(string message)
        : base(message)
    {
    }

    public LocationAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public static LocationAlreadyExistsException ForName(string name) =>
        new($"Location with name '{name}' already exists");
}
