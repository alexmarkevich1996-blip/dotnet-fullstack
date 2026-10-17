using DirectoryService.Web;
using DirectoryService.Web.Middlewares;
using Scalar.AspNetCore;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies(builder.Configuration);
builder.Services.AddSerilog((services, config) => config
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services));

WebApplication app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = ((diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestId", httpContext.TraceIdentifier);
    });
});
app.UseMiddleware<ExceptionMiddleware>();

app.MapGet("/", () => "Hello World!");
app.MapControllers();
app.MapHealthChecks("/health");

if (!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

await app.RunAsync();
