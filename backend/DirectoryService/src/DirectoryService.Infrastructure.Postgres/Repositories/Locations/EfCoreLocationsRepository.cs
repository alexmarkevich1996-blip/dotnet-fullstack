using CSharpFunctionalExtensions;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Common.Errors;
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

    public Task<Result<Guid, ErrorList>> AddAsync(Location location, CancellationToken cancellationToken)
    {
        _context.Locations.Add(location);
        return Task.FromResult(Result.Success<Guid, ErrorList>(location.Id));
    }

    public async Task<Location?> GetByNameAsync(Name name, CancellationToken cancellationToken)
    {
        return await _context.Locations
            .FirstOrDefaultAsync(l => l.Name == name, cancellationToken);
    }

    public Task<Guid> SaveAsync(Location location, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Not implemented yet");
    }

    public Task<Guid> DeleteAsync(Guid locationId, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Not implemented yet");
    }

    public async Task<Result<Location, ErrorList>> GetByIdAsync(Guid locationId, CancellationToken cancellationToken)
    {
        var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);

        if (location is null)
            return Error.NotFound("location.not_found", $"Location with id '{locationId}' was not found").ToErrors();

        return location;
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
