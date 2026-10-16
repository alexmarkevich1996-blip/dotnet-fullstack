using DirectoryService.Domain.Common.Errors;
using FluentValidation.Results;

namespace DirectoryService.Core.Validation;

public static class ValidationResultExtension
{
    public static ErrorList ToErrorList(this ValidationResult validationResult) =>
        ErrorList.From(validationResult.Errors
            .Select(failure => failure.CustomState as Error
                               ?? Error.Validation($"validation.{failure.PropertyName}", failure.ErrorMessage)));
}