using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Positions.UpdatePosition;

public partial class UpdatePositionCommandHandler
    : ICommandHandler<UpdatePositionCommand, UnitResult<ErrorList>>
{
    private readonly IPositionsRepository _positionsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<UpdatePositionCommandHandler> _logger;

    public UpdatePositionCommandHandler(
        IPositionsRepository positionsRepository,
        ITransactionManager transactionManager,
        ILogger<UpdatePositionCommandHandler> logger)
    {
        _positionsRepository = positionsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        UpdatePositionCommand command, CancellationToken cancellationToken)
    {
        var positionResult = await _positionsRepository.GetByIdAsync(command.PositionId, cancellationToken);
        if (positionResult.IsFailure)
            return positionResult.Error;

        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        positionResult.Value.UpdateName(nameResult.Value);

        var result = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (result.IsSuccess)
            LogPositionUpdated(command.PositionId);

        return result;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Position {PositionId} updated")]
    private partial void LogPositionUpdated(Guid positionId);
}
