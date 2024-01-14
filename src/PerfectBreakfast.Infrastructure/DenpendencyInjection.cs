﻿using Hangfire;
using Hangfire.Redis.StackExchange;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Infrastructure.MailServices;
using PerfectBreakfast.Infrastructure.Payments;
using System.Reflection;

namespace PerfectBreakfast.Infrastructure;

public static class DenpendencyInjection
{
    public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection, string redisConnection)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICurrentTime, CurrentTime>();
        services.AddTransient<JWTService>();
        services.AddTransient<IMailService, MailService>();
        // ATTENTION: if you do migration please check file README.md
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql(
                databaseConnection,
                new MySqlServerVersion(new Version(8, 2, 0)),
                mySqlOptions => mySqlOptions
                    .EnableRetryOnFailure());
        });

        services.AddHangfire(configuration => configuration.UseRedisStorage(redisConnection));
        JobStorage.Current = new RedisStorage(redisConnection);
        services.AddHangfireServer();

        // register Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddSingleton<ICurrentTime, CurrentTime>();
        services.AddScoped<IMapper, ServiceMapper>();

        // register service here
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IDeliveryUnitService, DeliveryUnitService>();
        services.AddScoped<IManagementUnitService, ManagementUnitService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IFoodService, FoodService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IPaymentMethodService, PaymentMethodService>();
        services.AddScoped<IComboService, ComboService>();
        services.AddScoped<IDailyOrderService, DailyOrderService>();
        services.AddScoped<IPayOsService, PayOsService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ISupplierCommissionRateService, SupplierCommissionRateService>();
        return services;
    }
}
