using DirectoryService.Contracts.Locations;

namespace DirectoryService.Core.Locations;

public interface ILocationsService
{
    public Task<Guid> Create(CreateLocationDto locationDto, CancellationToken cancellationToken);
    public Task Update(Guid locationId, UpdateLocationDto request, CancellationToken cancellationToken);
}