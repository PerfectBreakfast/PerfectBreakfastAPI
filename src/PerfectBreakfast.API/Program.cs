﻿using Hangfire;
using HangfireBasicAuthenticationFilter;
using OfficeOpenXml;
using PerfectBreakfast.API;
using PerfectBreakfast.API.Middlewares;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration.Get<AppConfiguration>();
builder.Services.AddInfrastructuresService(configuration!.DatabaseConnection, configuration.RedisConnection);
builder.Services.AddWebAPIService(configuration);
builder.Services.AddSingleton(configuration);

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "API V2");
    });
}
app.UseMiddleware<GlobalExceptionMiddleware>();
//app.UseMiddleware<PerformanceMiddleware>();

// todo authentication
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CorsPolicy");
app.MapControllers();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "My Website",
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter
        {
            User = "admin",
            Pass = "123456"
        }
    }
});
app.Run();
