using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Core.Locations;

public interface ILocationsRepository
{
    Task<Result<Guid, ErrorList>> AddAsync(Location location, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> UpdateAsync(Location location, CancellationToken cancellationToken);

    Task<Guid> SaveAsync(Location location, CancellationToken cancellationToken);

    Task<Guid> DeleteAsync(Guid locationId, CancellationToken cancellationToken);

    Task<Result<Location, ErrorList>> GetByIdAsync(Guid locationId, CancellationToken cancellationToken);

    Task<Location?> GetByNameAsync(Name name, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Guid>> GetExistingIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);
}