using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Departments.RemoveDepartmentPosition;

public record RemoveDepartmentPositionCommand(Guid DepartmentId, Guid PositionId) : ICommand<UnitResult<ErrorList>>;
