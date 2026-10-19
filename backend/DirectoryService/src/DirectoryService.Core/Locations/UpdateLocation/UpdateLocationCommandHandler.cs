using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations.UpdateLocation;

public partial class UpdateLocationCommandHandler
    : ICommandHandler<UpdateLocationCommand, UnitResult<ErrorList>>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<UpdateLocationCommandHandler> _logger;

    public UpdateLocationCommandHandler(
        ILocationsRepository locationsRepository,
        ILogger<UpdateLocationCommandHandler> logger)
    {
        _locationsRepository = locationsRepository;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        UpdateLocationCommand command, CancellationToken cancellationToken)
    {
        var locationResult = await _locationsRepository.GetByIdAsync(command.LocationId, cancellationToken);
        if (locationResult.IsFailure)
            return locationResult.Error;

        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var addressResult = Address.Create(
            $"{command.City}, {command.Street}, {command.House}, {command.Apartment}");
        if (addressResult.IsFailure)
            return addressResult.Error;

        locationResult.Value.UpdateDetails(nameResult.Value, addressResult.Value);

        var result = await _locationsRepository.UpdateAsync(locationResult.Value, cancellationToken);
        if (result.IsSuccess)
            LogLocationUpdated(command.LocationId);

        return result;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Location {LocationId} updated")]
    private partial void LogLocationUpdated(Guid locationId);
}