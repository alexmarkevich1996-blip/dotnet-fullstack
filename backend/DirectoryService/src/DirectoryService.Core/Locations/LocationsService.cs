using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations;

public partial class LocationsService : ILocationsService
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateLocationDto> _validator;
    private readonly ILogger<LocationsService> _logger;

    public LocationsService(
        ILocationsRepository locationsRepository,
        IValidator<CreateLocationDto> validator,
        ILogger<LocationsService> logger)
    {
        _locationsRepository = locationsRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Create(CreateLocationDto locationDto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(locationDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return ErrorList.From(validationResult.Errors
                .Select(e => Error.Validation($"validation.{e.PropertyName}", e.ErrorMessage)));
        }

        var nameResult = Name.Create(locationDto.Name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var addressResult = Address.Create(
            $"{locationDto.City}, {locationDto.Street}, {locationDto.House}, {locationDto.Apartment}");
        if (addressResult.IsFailure)
            return addressResult.Error;

        var existingLocation = await _locationsRepository.GetByNameAsync(nameResult.Value, cancellationToken);
        if (existingLocation != null)
        {
            return Error.Conflict("location.name.duplicate",
                $"Location with name '{locationDto.Name}' already exists").ToErrors();
        }

        var location = Location.Create(nameResult.Value, addressResult.Value);

        var saveResult = await _locationsRepository.AddAsync(location, cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        LogLocationCreated(location.Id);

        return location.Id;
    }

    public async Task<UnitResult<ErrorList>> Update(Guid locationId, UpdateLocationDto request, CancellationToken cancellationToken)
    {
        var locationResult = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);
        if (locationResult.IsFailure)
            return locationResult.Error;

        var nameResult = Name.Create(request.Name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var addressResult = Address.Create(
            $"{request.City}, {request.Street}, {request.House}, {request.Apartment}");
        if (addressResult.IsFailure)
            return addressResult.Error;

        locationResult.Value.UpdateDetails(nameResult.Value, addressResult.Value);

        return await _locationsRepository.UpdateAsync(locationResult.Value, cancellationToken);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Location created with id {LocationId}")]
    private partial void LogLocationCreated(Guid locationId);
}
