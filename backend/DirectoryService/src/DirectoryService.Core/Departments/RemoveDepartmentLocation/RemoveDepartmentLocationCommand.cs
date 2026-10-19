using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Departments.RemoveDepartmentLocation;

public record RemoveDepartmentLocationCommand(Guid DepartmentId, Guid LocationId)
    : ICommand<UnitResult<ErrorList>>;