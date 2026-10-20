using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Positions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Positions.CreatePosition;

public partial class CreatePositionCommandHandler
    : ICommandHandler<CreatePositionCommand, Result<Guid, ErrorList>>
{
    private readonly IPositionsRepository _positionsRepository;
    private readonly IValidator<CreatePositionCommand> _validator;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<CreatePositionCommandHandler> _logger;

    public CreatePositionCommandHandler(
        IPositionsRepository positionsRepository,
        IValidator<CreatePositionCommand> validator,
        ITransactionManager transactionManager,
        ILogger<CreatePositionCommandHandler> logger)
    {
        _positionsRepository = positionsRepository;
        _validator = validator;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        CreatePositionCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var existingPosition = await _positionsRepository.GetByNameAsync(nameResult.Value, cancellationToken);
        if (existingPosition != null)
        {
            LogPositionNameConflict(command.Name);
            return Error.Conflict("position.name.duplicate",
                $"Position with name '{command.Name}' already exists").ToErrors();
        }

        var position = Position.Create(nameResult.Value);

        var stageResult = await _positionsRepository.AddAsync(position, cancellationToken);
        if (stageResult.IsFailure)
            return stageResult.Error;

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        LogPositionCreated(position.Id);

        return position.Id;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Position created with id {PositionId}")]
    private partial void LogPositionCreated(Guid positionId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Attempted to create position with duplicate name {PositionName}")]
    private partial void LogPositionNameConflict(string positionName);
}
