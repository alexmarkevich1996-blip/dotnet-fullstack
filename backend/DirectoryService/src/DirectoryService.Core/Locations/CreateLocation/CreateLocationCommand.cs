using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Locations.CreateLocation;

public record CreateLocationCommand(
    string Name,
    string City,
    string Street,
    string House,
    string Apartment) : ICommand<Result<Guid, ErrorList>>;