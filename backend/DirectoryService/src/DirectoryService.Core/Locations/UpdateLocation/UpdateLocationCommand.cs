using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Locations.UpdateLocation;

public record UpdateLocationCommand(
    Guid LocationId,
    string Name,
    string City,
    string Street,
    string House,
    string Apartment) : ICommand<UnitResult<ErrorList>>;