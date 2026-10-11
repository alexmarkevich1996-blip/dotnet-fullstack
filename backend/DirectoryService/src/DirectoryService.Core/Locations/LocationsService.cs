using DirectoryService.Contracts.Locations;
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

    public async Task<Guid> Create(CreateLocationDto locationDto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(locationDto, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var existingLocation = await _locationsRepository.GetByNameAsync(locationDto.Name, cancellationToken);
        if (existingLocation != null)
        {
            throw LocationAlreadyExistsException.ForName(locationDto.Name);
        }
        
        var name = Name.Create(locationDto.Name);
        var address = Address.Create($"{locationDto.City}, {locationDto.Street}, {locationDto.House}, {locationDto.Apartment}");
        var location = Location.Create(name, address);

        await _locationsRepository.AddAsync(location, cancellationToken);

        LogLocationCreated(location.Id);

        return location.Id;
    }

    public async Task Update(Guid locationId, UpdateLocationDto request, CancellationToken cancellationToken)
    {
        var existingLocation = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);
        if (existingLocation == null)
            throw LocationNotFoundException.ForId(locationId);
        
        var updatedName = Name.Create(request.Name);
        var updatedAddress = Address.Create(
                $"{request.City}, {request.Street}, {request.House}, {request.Apartment}");
        existingLocation.UpdateDetails(updatedName, updatedAddress);

        await _locationsRepository.UpdateAsync(existingLocation, cancellationToken);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Location created with id {LocationId}")]
    private partial void LogLocationCreated(Guid locationId);
}