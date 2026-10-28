using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Core.Locations.GetLocationById;

public class GetLocationByIdQueryHandler : IQueryHandler<GetLocationByIdQuery, Result<LocationResponseDto, ErrorList>>
{
    private readonly IReadDbContext _readDbContext;

    public GetLocationByIdQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<LocationResponseDto, ErrorList>> Handle(GetLocationByIdQuery query,
        CancellationToken cancellationToken)
    {
        var dto = await _readDbContext.Locations
            .Where(l => l.Id == query.LocationId)
            .Select(l => new LocationResponseDto(
                l.Id,
                l.Name.Value,
                l.Address.Value,
                l.CreatedAt,
                l.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
            return Error.NotFound("location.not_found", $"Location with id '{query.LocationId}' was not found")
                .ToErrors();
        return dto;
    }

}