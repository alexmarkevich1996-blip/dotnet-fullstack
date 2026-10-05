using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Web;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Directory")
    ?? throw new InvalidOperationException("Connection string 'Directory' is not configured.");

builder.Services.AddDbContext<DirectoryServiceDbContext>(options =>
    options.UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention());

builder.Services.AddProgramDependencies();

WebApplication app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapControllers();
app.MapHealthChecks("/health");

if (!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

await app.RunAsync();
