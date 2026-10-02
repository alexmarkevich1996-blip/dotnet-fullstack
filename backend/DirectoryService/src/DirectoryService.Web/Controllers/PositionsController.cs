using DirectoryService.Contracts.Positions;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class PositionsController : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] CreatePositionDto request, CancellationToken cancellationToken)
    {
        Guid newPositionId = Guid.NewGuid();
        return CreatedAtAction(nameof(GetById), new { positionId = newPositionId }, newPositionId);
    }

    [HttpGet]
    public IActionResult Get([FromQuery] GetPositionDto request, CancellationToken cancellationToken)
    {
        return Ok(Array.Empty<object>());
    }

    [HttpGet("{positionId:guid}")]
    public IActionResult GetById([FromRoute] Guid positionId, CancellationToken cancellationToken)
    {
        return NotFound();
    }

    [HttpPut("{positionId:guid}")]
    public IActionResult Update([FromRoute] Guid positionId, [FromBody] UpdatePositionDto request,
        CancellationToken cancellationToken)
    {
        return NoContent();
    }

    [HttpDelete("{positionId:guid}")]
    public IActionResult Delete([FromRoute] Guid positionId, CancellationToken cancellationToken)
    {
        return NoContent();
    }
}
