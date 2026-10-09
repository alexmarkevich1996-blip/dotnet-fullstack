using Dapper;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class DapperLocationsRepository : ILocationsRepository
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<DapperLocationsRepository> _logger;

    public DapperLocationsRepository(NpgsqlDataSource dataSource, ILogger<DapperLocationsRepository> logger)
    {
        _dataSource = dataSource;
        _logger = logger;
    }
    public async Task<Guid> AddAsync(Location location, CancellationToken cancellationToken)
    {
        const string locationInsertSql = """
                                         INSERT INTO locations (id, name, address, created_at, updated_at) 
                                         VALUES (@Id, @Name, @Address, @CreatedAt, @UpdatedAt)   
                                         """;
        var locationsInsertParams = new
        {
            location.Id,
            Name = location.Name.Value,
            Address = location.Address.Value,
            location.CreatedAt,
            location.UpdatedAt
        };
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await connection.ExecuteAsync(new CommandDefinition(
                locationInsertSql,
                locationsInsertParams,
                cancellationToken: cancellationToken));
            return location.Id;
        }
        catch (NpgsqlException ex)
        {
            _logger.LogLocationSaveFailed(ex, location.Id);
            throw new InvalidOperationException($"Failed to save location {location.Id}.", ex);
        }
    }

    public async Task<Location?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT
                                id AS Id,
                                name as Name,
                                address as Address,
                                created_at AS CreatedAt,
                                updated_at AS UpdatedAt
                           FROM locations
                           WHERE name = @Name;
                           """;
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        LocationRow? row = await connection.QuerySingleOrDefaultAsync<LocationRow>(
            new CommandDefinition(
                sql,
                new { Name = name },
                cancellationToken: cancellationToken));
        
        if (row == null)
            return null;
        
        return Location.Rehydrate(
            row.Id,
            Name.Create(row.Name),
            Address.Create(row.Address),
            row.CreatedAt,
            row.UpdatedAt
        );
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