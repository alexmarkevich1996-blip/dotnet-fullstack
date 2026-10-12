using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Locations;

public class LocationAlreadyExistsException : DomainException
{
    private LocationAlreadyExistsException(Error error) : base(error)
    {
    }

    public static LocationAlreadyExistsException ForName(string name) =>
        new(Error.Conflict("location.name.duplicate", $"Location with name '{name}' already exists"));
}
