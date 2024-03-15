using System.Diagnostics;
using System.Reflection;
using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Net.payOS;
using PerfectBreakfast.API.Contracts.Commons;
using PerfectBreakfast.API.Middlewares;
using PerfectBreakfast.API.Services;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure;

namespace PerfectBreakfast.API;

public static class DependencyInjection
{
    public static IServiceCollection AddWebAPIService(this IServiceCollection services,AppConfiguration appConfiguration)
    {
        // ************ Config fluent validation but need to update new ways to handle this **************
        //==================================================================================================================================
        services.AddControllers();
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .Select(e => new ErrorValidator(e.Key, e.Value.Errors.First().ErrorMessage))
                    .ToList();

                return new BadRequestObjectResult(new ErrorResponse(400, "Bad Request", errors, DateTime.Now));
            };
        });
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareHandler>();
        //==================================================================================================================================
        services.AddApiVersioning(options =>
        {
            //options.ApiVersionReader = new UrlSegmentApiVersionReader();
            options.DefaultApiVersion = new ApiVersion(1,0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        }).AddApiExplorer(opt =>
        {
            opt.GroupNameFormat = "'v'VVV";
            opt.SubstituteApiVersionInUrl = true;
        });
        //==================================================================================================================================
        //for appear summary
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "PerfectBreakfast API",
                Description = "An ASP.NET Core Web API for P&B Project",
                //TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "PerfectBreakfast",
                    Url = new Uri("https://example.com/contact")
                }
            });
            
            options.SwaggerDoc("v2", new OpenApiInfo
            {
                Version = "v2",
                Title = "PerfectBreakfast API",
                Description = "An ASP.NET Core Web API for P&B Project",
                //TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "PerfectBreakfast",
                    Url = new Uri("https://example.com/contact")
                }
            });
            
            options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter the Bearer Authorization string as following: `Bearer {Generated-JWT-Token}`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
            
            
            // using System.Reflection;
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        //==================================================================================================================================
        // be able to Authenticate users by using JWT
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidAudience = appConfiguration.JwtSettings.Audience,
                ValidIssuer = appConfiguration.JwtSettings.Issuer,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appConfiguration.JwtSettings.SecretKey))
            };
        }).AddGoogle(option =>
        {
            // config Google 
            option.ClientId = appConfiguration.Google.ClientId;
            option.ClientSecret = appConfiguration.Google.ClientSecret;
        });
        //==================================================================================================================================
        /*services.AddIdentityApiEndpoints<User>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();*/
        // 2 ways
        services.AddIdentityCore<User>(options =>
            {
                // password configuration
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                
                // for email configuration
                //options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                
                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

            })
            .AddRoles<Role>()   // be able to add role
            .AddRoleManager<RoleManager<Role>>()  // be able to make use of Role Manager
            .AddEntityFrameworkStores<AppDbContext>()  // providing out context
            .AddSignInManager<SignInManager<User>>()  // make use of signin manager
            .AddUserManager<UserManager<User>>()  // make use of User Manager to create users
            .AddDefaultTokenProviders();  // be able to create tokens for email confirmation
        //==================================================================================================================================
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        //==================================================================================================================================
        PayOS payOS = new PayOS(appConfiguration.PayOSSettings.ClientId,appConfiguration.PayOSSettings.ApiKey,appConfiguration.PayOSSettings.CheckSumKey);
        services.AddSingleton(payOS);
        //==================================================================================================================================
        services.AddAuthorization(opt =>
        {
            opt.AddPolicy(ConstantRole.RequireDeliveryAdminRole, policy => policy.RequireRole(ConstantRole.DELIVERY_ADMIN));
            opt.AddPolicy(ConstantRole.RequirePartnerAdminRole, policy => policy.RequireRole(ConstantRole.PARTNER_ADMIN));
            opt.AddPolicy(ConstantRole.RequireSuperAdminRole, policy => policy.RequireRole(ConstantRole.SUPER_ADMIN));
            opt.AddPolicy(ConstantRole.RequireCustomerRole, policy => policy.RequireRole(ConstantRole.CUSTOMER));
            opt.AddPolicy(ConstantRole.RequireDeliveryStaffRole, policy => policy.RequireRole(ConstantRole.DELIVERY_STAFF));
            //opt.AddPolicy("AdminOrUser", policy => policy.RequireRole("ADMIN", "USER"));
        });
        //==================================================================================================================================
        services.AddHealthChecks();
        services.AddSingleton<GlobalExceptionMiddleware>();
        services.AddSingleton<PerformanceMiddleware>();
        services.AddSingleton<Stopwatch>();
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddHttpContextAccessor();
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMemoryCache();         // inject Memory Cache
        return services;
    }
}