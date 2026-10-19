using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Departments;
using DirectoryService.Core.Locations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Core;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddDirectoryService(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(CoreAssemblyMarker).Assembly);

        services.Scan(scan => scan
            .FromAssemblyOf<CoreAssemblyMarker>()
            .AddClasses(classes => classes.AssignableToAny(
                typeof(ICommandHandler<,>),
                typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddScoped<ILocationsService, LocationsService>();
        services.AddScoped<IDepartmentsService, DepartmentsService>();

        return services;
    }
}

/// <summary>
/// Non-static anchor type for assembly-scanning (<c>FromAssemblyOf&lt;T&gt;</c> requires a non-static type).
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "S2094:Classes should not be empty",
    Justification = "Deliberately empty: this is a scanning anchor for Scrutor's FromAssemblyOf<T>(), " +
                     "not a real type.")]
internal sealed class CoreAssemblyMarker;
