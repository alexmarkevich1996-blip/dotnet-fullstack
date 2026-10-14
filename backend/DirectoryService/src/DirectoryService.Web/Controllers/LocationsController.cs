using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Locations;
using DirectoryService.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Create([FromBody] CreateLocationDto request, CancellationToken cancellationToken)
    {
        var result = await _locationsService.Create(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToResponse();
    }

    [HttpPatch("{locationId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid locationId, [FromBody] UpdateLocationDto request,
        CancellationToken cancellationToken)
    {
        var result = await _locationsService.Update(locationId, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.Error.ToResponse();
    }

    [HttpGet]
    public IActionResult Get([FromQuery] GetLocationDto request, CancellationToken cancellationToken)
    {
        return Ok(Array.Empty<object>());
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
