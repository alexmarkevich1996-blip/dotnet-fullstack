using DirectoryService.Core.Locations;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class EfCoreLocationsRepository : ILocationsRepository
{
    public Task<Guid> AddAsync(Location location, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Not implemented yet");
    }
    
    public Task<Location?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Not implemented yet");

    }

    public Task<Guid> SaveAsync(Location location, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Not implemented yet");

    }

    public Task<Guid> DeleteAsync(Guid locationId, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Not implemented yet");
    }

    public Task<Location?> GetByIdAsync(Guid locationId, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Not implemented yet");
    }

    
}