using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Core.Departments.GetDepartmentById;

public class GetDepartmentByIdQueryHandler
    : IQueryHandler<GetDepartmentByIdQuery, Result<DepartmentResponseDto, ErrorList>>
{
    private readonly IReadDbContext _readDbContext;

    public GetDepartmentByIdQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<DepartmentResponseDto, ErrorList>> Handle(
        GetDepartmentByIdQuery query, CancellationToken cancellationToken)
    {
        var dto = await _readDbContext.Departments
            .Where(d => d.Id == query.DepartmentId)
            .Select(d => new DepartmentResponseDto(
                d.Id,
                d.Name.Value,
                d.Slug.Value,
                d.Path.Value,
                d.ParentId,
                d.CreatedAt,
                d.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
            return Error.NotFound("department.not_found", $"Department with id '{query.DepartmentId}' was not found")
                .ToErrors();

        return dto;
    }
}