using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("api/[controller]")]

public sealed class TestController : ControllerBase
{
    [HttpGet]

    public IActionResult GetInfo()
    {
        return Ok(new
        {
            message = "Test controller is working"
        });
    }
}