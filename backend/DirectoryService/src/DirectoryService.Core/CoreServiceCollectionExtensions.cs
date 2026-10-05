using DirectoryService.Core.Locations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Core;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddDirectoryService(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(CoreServiceCollectionExtensions).Assembly);

        services.AddScoped<ILocationsService, LocationsService>();

        return services;
    }
}
