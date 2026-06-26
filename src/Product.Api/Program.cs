using Serilog;
using Product.Api.Extensions;
using Product.Api.Middleware;
using Product.Api.Filters;
using Microsoft.EntityFrameworkCore;
using Product.Infrastructure.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting web host");

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    });

    builder.Services.AddResponseCompression();
    
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddApiVersioningSupport();
    builder.Services.AddSwaggerSupport();

    var app = builder.Build();

    app.UseResponseCompression();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) 
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Management API v1");
        });
    }

    app.UseHttpsRedirection();
    
    app.UseCors("CorsPolicy");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        int retries = 6;
        while (retries > 0)
        {
            try
            {
                Log.Information("Attempting database migration...");
                context.Database.Migrate();
                Log.Information("Database migration successful.");
                break;
            }
            catch (Exception ex)
            {
                retries--;
                Log.Warning("Database migration failed. Retries remaining: {Retries}. Error: {Message}", retries, ex.Message);
                if (retries == 0)
                {
                    Log.Fatal(ex, "Database migration failed. Exhausted all retries.");
                    throw;
                }
                System.Threading.Thread.Sleep(5000);
            }
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
public partial class Program { }
