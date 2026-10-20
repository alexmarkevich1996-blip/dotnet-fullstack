using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Positions.CreatePosition;
using DirectoryService.Core.Positions.DeletePosition;
using DirectoryService.Core.Positions.UpdatePosition;
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
public class PositionsController : ControllerBase
{
    private readonly ICommandHandler<CreatePositionCommand, Result<Guid, ErrorList>> _createHandler;
    private readonly ICommandHandler<UpdatePositionCommand, UnitResult<ErrorList>> _updateHandler;
    private readonly ICommandHandler<DeletePositionCommand, UnitResult<ErrorList>> _deleteHandler;

    public PositionsController(
        ICommandHandler<CreatePositionCommand, Result<Guid, ErrorList>> createHandler,
        ICommandHandler<UpdatePositionCommand, UnitResult<ErrorList>> updateHandler,
        ICommandHandler<DeletePositionCommand, UnitResult<ErrorList>> deleteHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> Create([FromBody] CreatePositionDto request, CancellationToken cancellationToken)
    {
        var command = new CreatePositionCommand(request.Name);
        var result = await _createHandler.Handle(command, cancellationToken);
        return result.ToApiResult(StatusCodes.Status201Created);
    }

    [HttpGet]
    public IResult Get([FromQuery] GetPositionDto request, CancellationToken cancellationToken)
    {
        return Results.Json(Envelope.Ok(Array.Empty<object>()));
    }

    [HttpGet("{positionId:guid}")]
    public IResult GetById([FromRoute] Guid positionId, CancellationToken cancellationToken)
    {
        var error = Error.NotFound("position.not_found", $"Position with id '{positionId}' was not found");
        return Results.Json(Envelope.Fail(error.ToErrors()), statusCode: StatusCodes.Status404NotFound);
    }

    [HttpPatch("{positionId:guid}")]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> Update([FromRoute] Guid positionId, [FromBody] UpdatePositionDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePositionCommand(positionId, request.Name);
        var result = await _updateHandler.Handle(command, cancellationToken);
        return result.ToApiResult();
    }

    [HttpDelete("{positionId:guid}")]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> Delete([FromRoute] Guid positionId, CancellationToken cancellationToken)
    {
        var command = new DeletePositionCommand(positionId);
        var result = await _deleteHandler.Handle(command, cancellationToken);
        return result.ToApiResult();
    }
}
