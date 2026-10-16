using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Validation;
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
    private readonly IValidator<GetLocationDto> _getValidator;
    private readonly ILogger<LocationsService> _logger;

    public LocationsService(
        ILocationsRepository locationsRepository,
        IValidator<CreateLocationDto> validator,
        IValidator<GetLocationDto> getValidator,
        ILogger<LocationsService> logger)
    {
        _locationsRepository = locationsRepository;
        _validator = validator;
        _getValidator = getValidator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Create(CreateLocationDto locationDto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(locationDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
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

    public async Task<Result<IReadOnlyCollection<object>, ErrorList>> GetAll(GetLocationDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _getValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        return Array.Empty<object>();
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Location created with id {LocationId}")]
    private partial void LogLocationCreated(Guid locationId);
}
