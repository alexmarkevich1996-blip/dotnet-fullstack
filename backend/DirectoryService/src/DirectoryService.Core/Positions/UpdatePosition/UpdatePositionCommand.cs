using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Positions.UpdatePosition;

public record UpdatePositionCommand(Guid PositionId, string Name) : ICommand<UnitResult<ErrorList>>;
