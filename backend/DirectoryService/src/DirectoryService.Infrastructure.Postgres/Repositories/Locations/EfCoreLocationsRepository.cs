using DirectoryService.Core.Locations;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Repositories.Locations;

public class EfCoreLocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _context;

    public EfCoreLocationsRepository(DirectoryServiceDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(Location location, CancellationToken cancellationToken)
    {
        _context.Locations.Add(location);

        await _context.SaveChangesAsync(cancellationToken);
        return location.Id;
    }

    public async Task UpdateAsync(Location location, CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Location?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        Name nameValue = Name.Create(name);

        return await _context.Locations
            .FirstOrDefaultAsync(
                l => l.Name == nameValue,
                cancellationToken);
    }

    public Task<Guid> SaveAsync(Location location, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Not implemented yet");

    }

    public Task<Guid> DeleteAsync(Guid locationId, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Not implemented yet");
    }

    public async Task<Location?> GetByIdAsync(Guid locationId, CancellationToken cancellationToken)
    {
        return await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Guid>> GetExistingIdsAsync(
        IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken)
    {
        return await _context.Locations
            .Where(l => ids.Contains(l.Id))
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);
    }
}