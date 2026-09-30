using DirectoryService.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("Directory")
    ?? throw new InvalidOperationException("Connection string 'Directory' is not configured.");

builder.Services.AddDbContext<DirectoryServiceDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

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
