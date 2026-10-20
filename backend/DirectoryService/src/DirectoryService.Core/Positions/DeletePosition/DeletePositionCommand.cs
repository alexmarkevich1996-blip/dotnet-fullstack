using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Positions.DeletePosition;

public record DeletePositionCommand(Guid PositionId) : ICommand<UnitResult<ErrorList>>;
