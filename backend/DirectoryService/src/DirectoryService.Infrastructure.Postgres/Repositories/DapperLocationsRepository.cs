using Dapper;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Npgsql;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class DapperLocationsRepository : ILocationsRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public DapperLocationsRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    public Task<Guid> AddAsync(Location location, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Not implemented yet");
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