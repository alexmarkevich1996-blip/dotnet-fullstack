using DirectoryService.Core;

namespace DirectoryService.Web;

public static class WebServiceCollectionExtensions
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services)
    {
        return services.AddWebDependencies()
            .AddDirectoryService();
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