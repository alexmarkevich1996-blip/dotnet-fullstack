using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using FluentValidation.Results;

namespace DirectoryService.Core.Locations;

public class CreateLocationValidator : AbstractValidator<CreateLocationDto>
{
    public CreateLocationValidator()
    {
        RuleFor(x => x.Name).MustBeValueObject(Name.Create);
        
        RuleFor(x => x.City).NotEmpty().WithMessage("City is not valid").MaximumLength(75).WithMessage("City is not valid");
        RuleFor(x => x.Street).NotEmpty().WithMessage("Street is not valid").MaximumLength(75).WithMessage("Street is not valid");
        RuleFor(x => x.House).NotEmpty().WithMessage("House is not valid").MaximumLength(75).WithMessage("House is not valid");
        RuleFor(x => x.Apartment).NotEmpty().WithMessage("Apartment is not valid").MaximumLength(75).WithMessage("Apartment is not valid");

        RuleFor(x => x).Custom((dto, context) =>
        {
            var result = Address.Create($"{dto.City}, {dto.Street}, {dto.House}, {dto.Apartment}");
            if (result.IsFailure)
            {
                foreach (var error in result.Error)
                {
                    context.AddFailure(new ValidationFailure(nameof(CreateLocationDto.City), error.Message)
                    {
                        CustomState = error
                    });
                }
            }
        });
    }
}