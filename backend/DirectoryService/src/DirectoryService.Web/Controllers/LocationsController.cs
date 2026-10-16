using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Locations;
using DirectoryService.Web.Contracts;
using DirectoryService.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationsService _locationsService;

    public LocationsController(ILocationsService locationsService)
    {
        _locationsService = locationsService;
    }
    [HttpPost]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> Create([FromBody] CreateLocationDto request, CancellationToken cancellationToken)
    {
        var result = await _locationsService.Create(request, cancellationToken);
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
        var result = await _locationsService.Update(locationId, request, cancellationToken);
        return result.ToApiResult();
    }

    [HttpGet]
    [ProducesResponseType(typeof(Envelope<IReadOnlyCollection<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Get([FromQuery] GetLocationDto request, CancellationToken cancellationToken)
    {
        var result = await _locationsService.GetAll(request, cancellationToken);
        return result.ToApiResult();
    }

    [HttpGet("{locationId:guid}")]
    public IActionResult GetById([FromRoute] Guid locationId, CancellationToken cancellationToken)
    {
        return NotFound();
    }

    [HttpDelete("{locationId:guid}")]
    public IActionResult Delete([FromRoute] Guid locationId, CancellationToken cancellationToken)
    {
        return NoContent();
    }
}
