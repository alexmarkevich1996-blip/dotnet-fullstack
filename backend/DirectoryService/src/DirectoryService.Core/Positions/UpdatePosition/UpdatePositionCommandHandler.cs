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

        var existingPosition = await _positionsRepository.GetByNameAsync(nameResult.Value, cancellationToken);
        if (existingPosition != null && existingPosition.Id != command.PositionId)
        {
            LogPositionNameConflict(command.Name);
            return Error.Conflict("position.name.duplicate",
                $"Position with name '{command.Name}' already exists").ToErrors();
        }

        positionResult.Value.UpdateName(nameResult.Value);

        var result = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (result.IsSuccess)
            LogPositionUpdated(command.PositionId);

        return result;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Position {PositionId} updated")]
    private partial void LogPositionUpdated(Guid positionId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Attempted to rename position to duplicate name {PositionName}")]
    private partial void LogPositionNameConflict(string positionName);
}
