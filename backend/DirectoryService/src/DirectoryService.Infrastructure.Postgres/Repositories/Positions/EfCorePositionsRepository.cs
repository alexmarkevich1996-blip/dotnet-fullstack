using CSharpFunctionalExtensions;
using DirectoryService.Core.Positions;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Repositories.Positions;

public class EfCorePositionsRepository : IPositionsRepository
{
    private readonly DirectoryServiceDbContext _context;

    public EfCorePositionsRepository(DirectoryServiceDbContext context)
    {
        _context = context;
    }

    public Task<Result<Guid, ErrorList>> AddAsync(Position position, CancellationToken cancellationToken)
    {
        _context.Positions.Add(position);
        return Task.FromResult(Result.Success<Guid, ErrorList>(position.Id));
    }

    public async Task<Result<Position, ErrorList>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var position = await _context.Positions
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (position is null)
            return Error.NotFound("position.not_found", $"Position with id '{id}' was not found").ToErrors();

        return position;
    }

    public async Task<Position?> GetByNameAsync(Name name, CancellationToken cancellationToken)
    {
        return await _context.Positions
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<bool> HasDepartmentLinksAsync(Guid positionId, CancellationToken cancellationToken)
    {
        return await _context.DepartmentPositions
            .AnyAsync(dp => dp.PositionId == positionId, cancellationToken);
    }

    public void Remove(Position position)
    {
        _context.Positions.Remove(position);
    }
}
