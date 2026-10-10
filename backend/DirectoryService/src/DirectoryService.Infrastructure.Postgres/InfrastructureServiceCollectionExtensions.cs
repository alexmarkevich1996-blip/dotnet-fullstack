using DirectoryService.Core.Departments;
using DirectoryService.Core.Locations;
using DirectoryService.Infrastructure.Postgres.Repositories;
using DirectoryService.Infrastructure.Postgres.Repositories.Departments;
using DirectoryService.Infrastructure.Postgres.Repositories.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace DirectoryService.Infrastructure.Postgres;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Directory")
                               ?? throw new InvalidOperationException("Connection string 'Directory' is not configured.");

        services.AddDbContext<DirectoryServiceDbContext>(options =>
            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention());

        services.AddNpgsqlDataSource(connectionString);

        bool useDapper = configuration.GetValue<bool>("UseDapperRepository");

        if (useDapper)
            services.AddScoped<ILocationsRepository, DapperLocationsRepository>();
        else
            services.AddScoped<ILocationsRepository, EfCoreLocationsRepository>();

        services.AddScoped<IDepartmentsRepository, EfCoreDepartmentsRepository>();

        return services;
    }
}