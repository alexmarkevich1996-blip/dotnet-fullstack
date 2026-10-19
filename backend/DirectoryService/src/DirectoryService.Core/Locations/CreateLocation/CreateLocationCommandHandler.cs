using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations.CreateLocation;

public partial class CreateLocationCommandHandler
    : ICommandHandler<CreateLocationCommand, Result<Guid, ErrorList>>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateLocationCommand> _validator;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<CreateLocationCommandHandler> _logger;

    public CreateLocationCommandHandler(
        ILocationsRepository locationsRepository,
        IValidator<CreateLocationCommand> validator,
        ITransactionManager transactionManager,
        ILogger<CreateLocationCommandHandler> logger)
    {
        _locationsRepository = locationsRepository;
        _validator = validator;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        CreateLocationCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var addressResult = Address.Create(
            $"{command.City}, {command.Street}, {command.House}, {command.Apartment}");
        if (addressResult.IsFailure)
            return addressResult.Error;

        var existingLocation = await _locationsRepository.GetByNameAsync(nameResult.Value, cancellationToken);
        if (existingLocation != null)
        {
            LogLocationNameConflict(command.Name);
            return Error.Conflict("location.name.duplicate",
                $"Location with name '{command.Name}' already exists").ToErrors();
        }

        var location = Location.Create(nameResult.Value, addressResult.Value);

        var stageResult = await _locationsRepository.AddAsync(location, cancellationToken);
        if (stageResult.IsFailure)
            return stageResult.Error;

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        LogLocationCreated(location.Id);

        return location.Id;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Location created with id {LocationId}")]
    private partial void LogLocationCreated(Guid locationId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Attempted to create location with duplicate name {LocationName}")]
    private partial void LogLocationNameConflict(string locationName);
}