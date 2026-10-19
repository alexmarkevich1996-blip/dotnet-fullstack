using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Locations.GetLocations;

public record GetLocationsQuery(string? Search, int Page, int PageSize)
    : IQuery<Result<IReadOnlyCollection<object>, ErrorList>>;