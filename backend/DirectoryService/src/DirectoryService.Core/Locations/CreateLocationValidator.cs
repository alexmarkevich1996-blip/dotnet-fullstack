using DirectoryService.Contracts.Locations;
using FluentValidation;

namespace DirectoryService.Core.Locations;

public class CreateLocationValidator : AbstractValidator<CreateLocationDto>
{
    public CreateLocationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(64)
            .WithMessage("Name is not valid");
        RuleFor(x => x.City).NotEmpty().MaximumLength(75).WithMessage("City is not valid");
        RuleFor(x => x.Street).NotEmpty().MaximumLength(75).WithMessage("Street is not valid");
        RuleFor(x => x.House).NotEmpty().MaximumLength(75).WithMessage("House is not valid");
        RuleFor(x => x.Apartment).NotEmpty().MaximumLength(75).WithMessage("Apartment is not valid");
        
    }
}