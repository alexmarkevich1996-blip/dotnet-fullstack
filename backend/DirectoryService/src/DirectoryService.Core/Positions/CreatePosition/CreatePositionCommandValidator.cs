using DirectoryService.Core.Validation;
using DirectoryService.Domain.Common.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Positions.CreatePosition;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        RuleFor(x => x.Name).MustBeValueObject(Name.Create);
    }
}
