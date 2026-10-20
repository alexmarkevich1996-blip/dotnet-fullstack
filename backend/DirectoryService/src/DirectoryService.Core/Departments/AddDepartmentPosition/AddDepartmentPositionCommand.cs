using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Departments.AddDepartmentPosition;

public record AddDepartmentPositionCommand(Guid DepartmentId, Guid PositionId) : ICommand<UnitResult<ErrorList>>;
