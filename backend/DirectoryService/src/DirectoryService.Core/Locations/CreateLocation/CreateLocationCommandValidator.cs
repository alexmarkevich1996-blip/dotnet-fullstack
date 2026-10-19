using DirectoryService.Core.Validation;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using FluentValidation.Results;

namespace DirectoryService.Core.Locations.CreateLocation;

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(x => x.Name).MustBeValueObject(Name.Create);

        RuleFor(x => x.City).NotEmpty().WithMessage("City is not valid").MaximumLength(75).WithMessage("City is not valid");
        RuleFor(x => x.Street).NotEmpty().WithMessage("Street is not valid").MaximumLength(75).WithMessage("Street is not valid");
        RuleFor(x => x.House).NotEmpty().WithMessage("House is not valid").MaximumLength(75).WithMessage("House is not valid");
        RuleFor(x => x.Apartment).NotEmpty().WithMessage("Apartment is not valid").MaximumLength(75).WithMessage("Apartment is not valid");

        RuleFor(x => x).Custom((command, context) =>
        {
            var result = Address.Create($"{command.City}, {command.Street}, {command.House}, {command.Apartment}");
            if (result.IsFailure)
            {
                foreach (var error in result.Error)
                {
                    context.AddFailure(new ValidationFailure(nameof(CreateLocationCommand.City), error.Message)
                    {
                        CustomState = error
                    });
                }
            }
        });
    }
}
