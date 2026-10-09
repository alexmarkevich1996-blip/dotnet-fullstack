using DirectoryService.Core;
using DirectoryService.Infrastructure.Postgres;
using Microsoft.Extensions.Configuration;

namespace DirectoryService.Web;

public static class WebServiceCollectionExtensions
{
    public static IServiceCollection AddProgramDependencies(
        this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddWebDependencies()
            .AddDirectoryService()
            .AddPostgresInfrastructure(configuration);
    }

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();
        services.AddHealthChecks();
        services.AddRouting(options => options.LowercaseUrls = true);
        
        return services;
    }
}