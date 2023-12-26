using PerfectBreakfast.API;
using PerfectBreakfast.API.Middlewares;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration.Get<AppConfiguration>();
builder.Services.AddInfrastructuresService(configuration!.DatabaseConnection);
builder.Services.AddWebAPIService(configuration.JwtSettings);
builder.Services.AddSingleton(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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
app.UseAuthorization();

app.MapControllers();

app.Run();
