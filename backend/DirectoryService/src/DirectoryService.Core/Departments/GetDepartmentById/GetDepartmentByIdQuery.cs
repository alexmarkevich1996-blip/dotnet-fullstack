using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Departments.GetDepartmentById;

public record GetDepartmentByIdQuery(Guid DepartmentId)
    : IQuery<Result<DepartmentResponseDto, ErrorList>>;