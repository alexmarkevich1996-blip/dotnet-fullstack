using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Departments.CreateDepartment;

public record CreateDepartmentCommand(
    string Name,
    string Slug,
    Guid? ParentId,
    IReadOnlyCollection<Guid> LocationIds) : ICommand<Result<Guid, ErrorList>>;