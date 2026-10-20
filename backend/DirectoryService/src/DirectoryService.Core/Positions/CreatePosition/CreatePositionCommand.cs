using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Positions.CreatePosition;

public record CreatePositionCommand(string Name) : ICommand<Result<Guid, ErrorList>>;
