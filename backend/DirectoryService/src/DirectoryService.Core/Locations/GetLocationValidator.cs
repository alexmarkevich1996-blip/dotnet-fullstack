using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Common.Errors;
using FluentValidation;

namespace DirectoryService.Core.Locations;

public class GetLocationValidator : AbstractValidator<GetLocationDto>
{
    public GetLocationValidator()
    {
        RuleFor(x => x.Page)
            .MustSatisfy(
                page => page >= 1,
                page => Error.Validation("query.page_invalid", $"Page must be greater than or equal to 1, got {page}"));

        RuleFor(x => x.PageSize)
            .MustSatisfy(
                pageSize => pageSize is >= 1 and <= 100,
                pageSize => Error.Validation("query.pageSize_invalid",
                    $"PageSize must be between 1 and 100, got {pageSize}"));
    }
}