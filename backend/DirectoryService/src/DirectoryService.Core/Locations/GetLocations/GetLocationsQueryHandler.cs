using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Common.Errors;
using FluentValidation;

namespace DirectoryService.Core.Locations.GetLocations;

public class GetLocationsQueryHandler
    : IQueryHandler<GetLocationsQuery, Result<IReadOnlyCollection<object>, ErrorList>>
{
    private readonly IValidator<GetLocationsQuery> _validator;

    public GetLocationsQueryHandler(IValidator<GetLocationsQuery> validator)
    {
        _validator = validator;
    }

    public async Task<Result<IReadOnlyCollection<object>, ErrorList>> Handle(
        GetLocationsQuery query, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        return Array.Empty<object>();
    }
}