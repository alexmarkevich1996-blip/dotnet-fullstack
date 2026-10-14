using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments;
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
    public async Task<IActionResult> Create([FromBody] CreateDepartmentDto request, CancellationToken cancellationToken)
    {
        var result = await _departmentsService.Create(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToResponse();
    }

    [HttpPost("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<IActionResult> AddLocation([FromRoute] Guid departmentId, [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.AddLocation(departmentId, locationId, cancellationToken);
        return result.IsSuccess ? NoContent() : result.Error.ToResponse();
    }

    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<IActionResult> RemoveLocation([FromRoute] Guid departmentId, [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.RemoveLocation(departmentId, locationId, cancellationToken);
        return result.IsSuccess ? NoContent() : result.Error.ToResponse();
    }

    [HttpPatch("{departmentId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid departmentId, [FromBody] UpdateDepartmentDto request,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.Update(departmentId, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.Error.ToResponse();
    }

    [HttpGet]
    public IActionResult Get([FromQuery] GetDepartmentDto request, CancellationToken cancellationToken)
    {
        return Ok(Array.Empty<object>());
    }
    
    [HttpGet("{departmentId:guid}")]
    public IActionResult GetById([FromRoute] Guid departmentId, CancellationToken cancellationToken)
    {
        return NotFound();
    }

    [HttpDelete("{departmentId:guid}")]
    public IActionResult Delete([FromRoute] Guid departmentId, CancellationToken cancellationToken)
    {
        return NoContent();
    }
}