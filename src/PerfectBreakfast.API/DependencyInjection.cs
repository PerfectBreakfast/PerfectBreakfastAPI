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
using PerfectBreakfast.API.Contracts.Commons;
using PerfectBreakfast.API.Middlewares;
using PerfectBreakfast.API.Services;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Infrastructure;

namespace PerfectBreakfast.API;

public static class DependencyInjection
{
    public static IServiceCollection AddWebAPIService(this IServiceCollection services,JwtSettings jwtSettings )
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
        /*services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });*/
        services.AddAuthentication();
        //==================================================================================================================================
        services.AddIdentityApiEndpoints<User>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
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
        return services;
    }
}