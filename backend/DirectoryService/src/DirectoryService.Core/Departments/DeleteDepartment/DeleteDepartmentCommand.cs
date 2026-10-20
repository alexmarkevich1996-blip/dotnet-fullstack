using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Departments.DeleteDepartment;

public record DeleteDepartmentCommand(Guid DepartmentId) : ICommand<UnitResult<ErrorList>>;
