namespace DirectoryService.Infrastructure.Postgres.Repositories.Locations;

internal sealed record LocationRow(Guid Id, string Name, string Address, DateTime CreatedAt, DateTime UpdatedAt);