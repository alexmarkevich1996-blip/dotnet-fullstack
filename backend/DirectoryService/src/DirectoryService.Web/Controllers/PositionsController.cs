using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Positions;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Web.Contracts;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class PositionsController : ControllerBase
{
    [HttpPost]
    public IResult Create([FromBody] CreatePositionDto request, CancellationToken cancellationToken)
    {
        Guid newPositionId = Guid.NewGuid();
        return Results.Json(Envelope.Ok(newPositionId), statusCode: StatusCodes.Status201Created);
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

    [HttpPut("{positionId:guid}")]
    public IResult Update([FromRoute] Guid positionId, [FromBody] UpdatePositionDto request,
        CancellationToken cancellationToken)
    {
        return Results.Json(Envelope.Ok<object?>(null));
    }

    [HttpDelete("{positionId:guid}")]
    public IResult Delete([FromRoute] Guid positionId, CancellationToken cancellationToken)
    {
        return Results.Json(Envelope.Ok<object?>(null));
    }
}
