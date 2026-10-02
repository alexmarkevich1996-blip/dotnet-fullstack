using DirectoryService.Contracts.Departments;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class DepartmentsController : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] CreateDepartmentDto request, CancellationToken cancellationToken)
    {
        Guid newDepartmentId = Guid.NewGuid();
        return CreatedAtAction(nameof(GetById), new { departmentId = newDepartmentId }, newDepartmentId);   
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

    [HttpPut("{departmentId:guid}")]
    public IActionResult Update([FromRoute] Guid departmentId, [FromBody] UpdateDepartmentDto request,
        CancellationToken cancellationToken)
    {
        return NoContent();
    }

    [HttpDelete("{departmentId:guid}")]
    public IActionResult Delete([FromRoute] Guid departmentId, CancellationToken cancellationToken)
    {
        return NoContent();
    }
}