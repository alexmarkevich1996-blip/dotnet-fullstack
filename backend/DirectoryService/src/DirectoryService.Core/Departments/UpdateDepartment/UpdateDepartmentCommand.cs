using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Departments.UpdateDepartment;

public record UpdateDepartmentCommand(Guid DepartmentId, string Name)
    : ICommand<UnitResult<ErrorList>>;