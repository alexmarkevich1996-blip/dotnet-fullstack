using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Positions;

namespace DirectoryService.Core.Positions;

public interface IPositionsRepository
{
    Task<Result<Guid, ErrorList>> AddAsync(Position position, CancellationToken cancellationToken);
    Task<Result<Position, ErrorList>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Position?> GetByNameAsync(Name name, CancellationToken cancellationToken);
    Task<bool> HasDepartmentLinksAsync(Guid positionId, CancellationToken cancellationToken);
    void Remove(Position position);
}
