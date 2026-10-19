using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Departments.AddDepartmentLocation;

public record AddDepartmentLocationCommand(Guid DepartmentId, Guid LocationId)
    : ICommand<UnitResult<ErrorList>>;
