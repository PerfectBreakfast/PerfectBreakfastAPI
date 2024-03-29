﻿using Hangfire;
using Hangfire.Storage.SQLite;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Infrastructure.BackgroundJobServices;
using PerfectBreakfast.Infrastructure.ImgurServices;
using PerfectBreakfast.Infrastructure.MailServices;
using PerfectBreakfast.Infrastructure.Payments;
using System.Reflection;
using PerfectBreakfast.Infrastructure.ExportService;

namespace PerfectBreakfast.Infrastructure;

public static class DenpendencyInjection
{
    public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection, string redisConnection)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICurrentTime, CurrentTime>();
        services.AddTransient<IJwtService,JWTService>();
        services.AddTransient<IMailService, MailService>();
        services.AddScoped<IManagementService, ManagementService>();
        services.AddScoped<IImgurService, ImgurService>();
        services.AddScoped<IExportExcelService, ExportExcelService>();

        // ATTENTION: if you do migration please check file README.md
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql(
                databaseConnection,
                new MySqlServerVersion(new Version(8, 2, 0)),
                mySqlOptions => mySqlOptions
                    .EnableRetryOnFailure());
        });
        
        // đăng kí redis 
        services.AddStackExchangeRedisCache(opt =>
        {
            opt.Configuration = redisConnection;
        });

        // register hangfire 
        services.AddHangfire(hangfire =>
        {
            hangfire.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
            hangfire.UseSimpleAssemblyNameTypeSerializer();
            hangfire.UseRecommendedSerializerSettings();
            hangfire.UseColouredConsoleLogProvider();
            hangfire.UseSQLiteStorage("Hangfire.db"); // storage by SQLite
        });
        services.AddHangfireServer();

        // register Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        // register service here
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IDeliveryService, DeliveryService>();
        services.AddScoped<IPartnerService, PartnerService>();
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
        services.AddScoped<ISupplyAssigmentService, SupplyAssigmentService>();
        services.AddScoped<ISupplierFoodAssignmentService, SupplierFoodAssignmentService>();
        services.AddScoped<IShippingOrderService, ShippingOrderService>();
        services.AddScoped<IHangfireSettingService, HangFireSettingService>();
        services.AddScoped<IMealService, MealService>();
        return services;
    }
}
