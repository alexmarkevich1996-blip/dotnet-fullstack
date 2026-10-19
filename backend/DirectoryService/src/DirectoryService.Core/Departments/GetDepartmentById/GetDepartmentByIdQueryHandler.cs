using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Core.Departments.GetDepartmentById;

public class GetDepartmentByIdQueryHandler
    : IQueryHandler<GetDepartmentByIdQuery, Result<DepartmentResponseDto, ErrorList>>
{
    private readonly IDepartmentsRepository _departmentsRepository;

    public GetDepartmentByIdQueryHandler(IDepartmentsRepository departmentsRepository)
    {
        _departmentsRepository = departmentsRepository;
    }

    public async Task<Result<DepartmentResponseDto, ErrorList>> Handle(
        GetDepartmentByIdQuery query, CancellationToken cancellationToken)
    {
        var departmentResult = await _departmentsRepository.GetByIdAsync(query.DepartmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        var department = departmentResult.Value;

        return new DepartmentResponseDto(
            department.Id,
            department.Name.Value,
            department.Slug.Value,
            department.Path.Value,
            department.ParentId,
            department.CreatedAt,
            department.UpdatedAt);
    }
}