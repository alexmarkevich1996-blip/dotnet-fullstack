using DirectoryService.Core.Validation;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Departments.CreateDepartment;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.Name).MustBeValueObject(Name.Create);
        RuleFor(x => x.Slug).MustBeValueObject(Slug.Create);
    }
}