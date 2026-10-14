using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Locations;

public interface ILocationsService
{
    public Task<Result<Guid, ErrorList>> Create(CreateLocationDto locationDto, CancellationToken cancellationToken);
    public Task<UnitResult<ErrorList>> Update(Guid locationId, UpdateLocationDto request, CancellationToken cancellationToken);
}