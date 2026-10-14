using System.Diagnostics.CodeAnalysis;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Web.Extensions;

namespace DirectoryService.Web.Middlewares;

public partial class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    [SuppressMessage(
        "Design",
        "CA1031:Do not catch general exception types",
        Justification = "This is the top-level exception-handling middleware for the whole request pipeline " +
                         "(the 'roof'); it must catch everything that was not a known DomainException, log it, " +
                         "and return a generic 500 without leaking internals.")]
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            LogDomainError(ex, ex.Error.Code);

            context.Response.StatusCode = ex.Error.Type.ToStatusCode();
            await context.Response.WriteAsJsonAsync(ex.Error);
        }
        catch (Exception ex)
        {
            LogUnhandledException(ex);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = "Internal server error" });
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Domain error: {Code}")]
    private partial void LogDomainError(Exception exception, string code);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception")]
    private partial void LogUnhandledException(Exception exception);

}