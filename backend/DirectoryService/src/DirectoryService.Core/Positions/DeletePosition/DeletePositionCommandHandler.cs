using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Positions.DeletePosition;

public partial class DeletePositionCommandHandler
    : ICommandHandler<DeletePositionCommand, UnitResult<ErrorList>>
{
    private readonly IPositionsRepository _positionsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<DeletePositionCommandHandler> _logger;

    public DeletePositionCommandHandler(
        IPositionsRepository positionsRepository,
        ITransactionManager transactionManager,
        ILogger<DeletePositionCommandHandler> logger)
    {
        _positionsRepository = positionsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        DeletePositionCommand command, CancellationToken cancellationToken)
    {
        var positionResult = await _positionsRepository.GetByIdAsync(command.PositionId, cancellationToken);
        if (positionResult.IsFailure)
            return positionResult.Error;

        _positionsRepository.Remove(positionResult.Value);

        var result = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (result.IsSuccess)
            LogPositionDeleted(command.PositionId);

        return result;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Position {PositionId} deleted")]
    private partial void LogPositionDeleted(Guid positionId);
}
