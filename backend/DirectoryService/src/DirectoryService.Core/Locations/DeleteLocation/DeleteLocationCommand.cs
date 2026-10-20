using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Locations.DeleteLocation;

public record DeleteLocationCommand(Guid LocationId) : ICommand<UnitResult<ErrorList>>;
