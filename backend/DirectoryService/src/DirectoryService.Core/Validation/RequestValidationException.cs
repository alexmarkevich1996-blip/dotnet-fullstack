using DirectoryService.Domain.Common.Errors;
using FluentValidation.Results;

namespace DirectoryService.Core.Validation;

public class RequestValidationException : DomainException
{
    private RequestValidationException(Error error) : base(error)
    {
    }

    public static RequestValidationException FromValidationErrors(IEnumerable<ValidationFailure> errors)
    {
        string message = string.Join("; ", errors.Select(e => e.ErrorMessage));
        return new(Error.Validation("request.validation_failed", message));
    }
}
