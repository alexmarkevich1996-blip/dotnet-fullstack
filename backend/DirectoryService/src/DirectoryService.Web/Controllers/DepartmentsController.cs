using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments;
using DirectoryService.Web.Contracts;
using DirectoryService.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentsService _departmentsService;
    public DepartmentsController(IDepartmentsService departmentsService)
    {
        _departmentsService = departmentsService;
    }
    [HttpPost]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> Create([FromBody] CreateDepartmentDto request, CancellationToken cancellationToken)
    {
        var result = await _departmentsService.Create(request, cancellationToken);
        return result.ToApiResult(StatusCodes.Status201Created);
    }

    [HttpPost("{departmentId:guid}/locations/{locationId:guid}")]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> AddLocation([FromRoute] Guid departmentId, [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.AddLocation(departmentId, locationId, cancellationToken);
        return result.ToApiResult();
    }

    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> RemoveLocation([FromRoute] Guid departmentId, [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.RemoveLocation(departmentId, locationId, cancellationToken);
        return result.ToApiResult();
    }

    [HttpPatch("{departmentId:guid}")]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> Update([FromRoute] Guid departmentId, [FromBody] UpdateDepartmentDto request,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.Update(departmentId, request, cancellationToken);
        return result.ToApiResult();
    }

    [HttpGet("{departmentId:guid}")]
    [ProducesResponseType(typeof(Envelope<DepartmentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    public async Task<IResult> GetById([FromRoute] Guid departmentId, CancellationToken cancellationToken)
    {
        var result = await _departmentsService.GetById(departmentId, cancellationToken);
        return result.ToApiResult();
    }
    
    [HttpGet]
    public IActionResult Get([FromQuery] GetDepartmentDto request, CancellationToken cancellationToken)
    {
        return Ok(Array.Empty<object>());
    }

    [HttpDelete("{departmentId:guid}")]
    public IActionResult Delete([FromRoute] Guid departmentId, CancellationToken cancellationToken)
    {
        return NoContent();
    }
}