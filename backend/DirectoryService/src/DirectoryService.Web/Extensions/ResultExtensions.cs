using DirectoryService.Domain.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToResponse(this ErrorList errors)
    {
        int statusCode = errors.Count > 0
            ? errors[0].Type.ToStatusCode()
            : StatusCodes.Status500InternalServerError;

        return new ObjectResult(errors) { StatusCode = statusCode };
    }
}
