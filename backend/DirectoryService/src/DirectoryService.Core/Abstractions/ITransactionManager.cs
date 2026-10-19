using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Abstractions;

public interface ITransactionManager
{
    Task<UnitResult<ErrorList>> SaveChangesAsync(CancellationToken cancellationToken);

    Task<Result<T, ErrorList>> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<Result<T, ErrorList>>> operation,
        CancellationToken cancellationToken);

    Task<UnitResult<ErrorList>> ExecuteInTransactionAsync(
        Func<CancellationToken, Task<UnitResult<ErrorList>>> operation,
        CancellationToken cancellationToken);
}