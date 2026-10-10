using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments;
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
        var departmentId = await _departmentsService.Create(request, cancellationToken);
        return Ok(departmentId);
    }
    
    [HttpPatch("{departmentId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid departmentId, [FromBody] UpdateDepartmentDto request,
        CancellationToken cancellationToken)
    {
        await _departmentsService.Update(departmentId, request, cancellationToken);
        return NoContent();
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