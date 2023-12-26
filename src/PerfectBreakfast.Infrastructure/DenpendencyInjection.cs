using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PerfectBreakfast.Application.Interfaces;
using System.Reflection;
using PerfectBreakfast.Application.Services;

namespace PerfectBreakfast.Infrastructure;

public static class DenpendencyInjection
{
    public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICurrentTime, CurrentTime>();

        services.AddScoped<IUserService, UserService>();
        
        // ATTENTION: if you do migration please check file README.md
        services.AddDbContext<AppDbContext>(options => {
            options.UseMySql(
                databaseConnection, 
                new MySqlServerVersion(new Version(8, 2, 0)),
                mySqlOptions => mySqlOptions
                    .EnableRetryOnFailure()); 
        });
        
        // register Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
