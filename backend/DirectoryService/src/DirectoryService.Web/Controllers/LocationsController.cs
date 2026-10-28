using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Locations.CreateLocation;
using DirectoryService.Core.Locations.DeleteLocation;
using DirectoryService.Core.Locations.GetLocationById;
using DirectoryService.Core.Locations.GetLocations;
using DirectoryService.Core.Locations.UpdateLocation;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Web.Contracts;
using DirectoryService.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("[controller]")]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "S6960:Controllers should not have too many responsibilities",
    Justification = "Each injected handler encapsulates exactly one CQRS use case; the " +
                     "controller stays a thin HTTP boundary that only dispatches to them, it does not " +
                     "itself mix responsibilities.")]
public class LocationsController : ControllerBase
{
    private readonly ICommandHandler<CreateLocationCommand, Result<Guid, ErrorList>> _createHandler;
    private readonly ICommandHandler<UpdateLocationCommand, UnitResult<ErrorList>> _updateHandler;
    private readonly ICommandHandler<DeleteLocationCommand, UnitResult<ErrorList>> _deleteHandler;
    private readonly IQueryHandler<GetLocationsQuery, Result<IReadOnlyCollection<object>, ErrorList>> _getAllHandler;
    private readonly IQueryHandler<GetLocationByIdQuery, Result<LocationResponseDto, ErrorList>> _getByIdHandler;

    public LocationsController(
        ICommandHandler<CreateLocationCommand, Result<Guid, ErrorList>> createHandler,
        ICommandHandler<UpdateLocationCommand, UnitResult<ErrorList>> updateHandler,
        ICommandHandler<DeleteLocationCommand, UnitResult<ErrorList>> deleteHandler,
        IQueryHandler<GetLocationsQuery, Result<IReadOnlyCollection<object>, ErrorList>> getAllHandler,
        IQueryHandler<GetLocationByIdQuery, Result<LocationResponseDto, ErrorList>> getByIdHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> Create([FromBody] CreateLocationDto request, CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(request.Name, request.City, request.Street, request.House, request.Apartment);
        var result = await _createHandler.Handle(command, cancellationToken);
        return result.ToApiResult(StatusCodes.Status201Created);
    }

    [HttpPatch("{locationId:guid}")]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> Update([FromRoute] Guid locationId, [FromBody] UpdateLocationDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationCommand(locationId, request.Name, request.City, request.Street, request.House, request.Apartment);
        var result = await _updateHandler.Handle(command, cancellationToken);
        return result.ToApiResult();
    }

    [HttpGet]
    [ProducesResponseType(typeof(Envelope<IReadOnlyCollection<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Get([FromQuery] GetLocationDto request, CancellationToken cancellationToken)
    {
        var query = new GetLocationsQuery(request.Search, request.Page, request.PageSize);
        var result = await _getAllHandler.Handle(query, cancellationToken);
        return result.ToApiResult();
    }

    [HttpGet("{locationId:guid}")]
    [ProducesResponseType(typeof(Envelope<LocationResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    public async Task<IResult> GetById([FromRoute] Guid locationId, CancellationToken cancellationToken)
    {
        var query = new GetLocationByIdQuery(locationId);
        var result = await _getByIdHandler.Handle(query, cancellationToken);
        return result.ToApiResult();
    }

    [HttpDelete("{locationId:guid}")]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> Delete([FromRoute] Guid locationId, CancellationToken cancellationToken)
    {
        var command = new DeleteLocationCommand(locationId);
        var result = await _deleteHandler.Handle(command, cancellationToken);
        return result.ToApiResult();
    }
}