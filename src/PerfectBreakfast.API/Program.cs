﻿using Hangfire;
using PerfectBreakfast.API;
using PerfectBreakfast.API.Middlewares;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration.Get<AppConfiguration>();
builder.Services.AddInfrastructuresService(configuration!.DatabaseConnection, configuration.RedisConnection);
builder.Services.AddWebAPIService(configuration);
builder.Services.AddSingleton(configuration);

RecurringJob.AddOrUpdate<IDailyOrderService>(d => d.AutoUpdate(DateTime.UtcNow.AddHours(7)), Cron.Daily(9));

var app = builder.Build();

//app.MapGroup("/account").MapIdentityApi<User>();
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
app.UseMiddleware<PerformanceMiddleware>();

// todo authentication
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.MapControllers();

app.UseHangfireDashboard("/hangfire");

app.Run();
