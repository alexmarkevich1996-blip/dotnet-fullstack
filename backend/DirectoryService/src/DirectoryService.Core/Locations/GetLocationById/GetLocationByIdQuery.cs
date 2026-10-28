using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Locations.GetLocationById;

public record GetLocationByIdQuery(Guid LocationId) : IQuery<Result<LocationResponseDto, ErrorList>>;