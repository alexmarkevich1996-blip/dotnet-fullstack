using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Web.Contracts;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace DirectoryService.Web.Extensions;

public static class ResultExtensions
{
    public static IResult ToApiResult<T>(this Result<T, ErrorList> result, int successStatusCode = StatusCodes.Status200OK)
        => result.IsSuccess
            ? Results.Json(Envelope.Ok(result.Value), statusCode: successStatusCode)
            : result.Error.ToFailureResult();

    public static IResult ToApiResult(this UnitResult<ErrorList> result,
        int successStatusCode = StatusCodes.Status200OK)
        => result.IsSuccess
            ? Results.Json(Envelope.Ok<object?>(null), statusCode: successStatusCode)
            : result.Error.ToFailureResult();

    private static IResult ToFailureResult(this ErrorList errors)
    {
        int statusCode = errors.Count > 0 ? errors[0].Type.ToStatusCode() : StatusCodes.Status500InternalServerError;
        return Results.Json(Envelope.Fail(errors), statusCode: statusCode);
    }
}
