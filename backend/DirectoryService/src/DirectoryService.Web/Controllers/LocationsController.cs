using DirectoryService.Contracts.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] CreateLocationDto request, CancellationToken cancellationToken)
    {
        Guid newLocationId = Guid.NewGuid();
        return CreatedAtAction(nameof(GetById), new { locationId = newLocationId }, newLocationId);
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

    [HttpPut("{locationId:guid}")]
    public IActionResult Update([FromRoute] Guid locationId, [FromBody] UpdateLocationDto request,
        CancellationToken cancellationToken)
    {
        return NoContent();
    }

    [HttpDelete("{locationId:guid}")]
    public IActionResult Delete([FromRoute] Guid locationId, CancellationToken cancellationToken)
    {
        return NoContent();
    }
}
