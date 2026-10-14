using CSharpFunctionalExtensions;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories.Locations;

public partial class EfCoreLocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _context;
    private readonly ILogger<EfCoreLocationsRepository> _logger;

    public EfCoreLocationsRepository(DirectoryServiceDbContext context, ILogger<EfCoreLocationsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> AddAsync(Location location, CancellationToken cancellationToken)
    {
        _context.Locations.Add(location);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return location.Id;
        }
        catch (DbUpdateException ex)
        {
            LogSaveFailed(ex, location.Id);
            return Error.Internal("location.save_failed", "Failed to save location").ToErrors();
        }
    }

    public async Task<UnitResult<ErrorList>> UpdateAsync(Location location, CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<ErrorList>();
        }
        catch (DbUpdateException ex)
        {
            LogSaveFailed(ex, location.Id);
            return Error.Internal("location.save_failed", "Failed to save location").ToErrors();
        }
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

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to save location {LocationId}")]
    private partial void LogSaveFailed(Exception exception, Guid locationId);
}
