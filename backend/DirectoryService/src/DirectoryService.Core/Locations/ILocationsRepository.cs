using DirectoryService.Domain.Locations;

namespace DirectoryService.Core.Locations;

public interface ILocationsRepository
{
    Task<Guid> AddAsync(Location location, CancellationToken cancellationToken);
    Task UpdateAsync(Location location, CancellationToken cancellationToken);
    
    Task<Guid> SaveAsync(Location location, CancellationToken cancellationToken);
    
    Task<Guid> DeleteAsync(Guid locationId, CancellationToken cancellationToken);
    
    Task<Location?> GetByIdAsync(Guid locationId, CancellationToken cancellationToken);
    
    Task<Location?> GetByNameAsync(string name, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Guid>> GetExistingIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);
}