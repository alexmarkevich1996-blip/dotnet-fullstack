using DirectoryService.Contracts.Departments;
using FluentValidation;

namespace DirectoryService.Core.Departments;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentValidator()
    {
        RuleFor(d => d.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(64)
            .WithMessage("Name is not valid");
        RuleFor(d => d.Slug)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100)
            .WithMessage("Slug is not valid");
    }
}