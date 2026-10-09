namespace DirectoryService.Infrastructure.Postgres.Repositories;

internal sealed record LocationRow(Guid Id, string Name, string Address, DateTime CreatedAt, DateTime UpdatedAt);