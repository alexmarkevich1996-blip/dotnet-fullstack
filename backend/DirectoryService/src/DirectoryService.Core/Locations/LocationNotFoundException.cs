using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Locations;

public class LocationNotFoundException : DomainException
{
    private LocationNotFoundException(Error error) : base(error)
    {
    }

    public static LocationNotFoundException ForId(Guid id) =>
        new(Error.NotFound("location.not_found", $"Location '{id}' was not found"));

    public static LocationNotFoundException ForIds(IReadOnlyCollection<Guid> ids) =>
        new(Error.NotFound("location.not_found", $"Locations not found: {string.Join(", ", ids)}"));
}
