using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using ServiceConfigManager.Api.Configuration;
using ServiceConfigManager.Api.Extensions;
using ServiceConfigManager.Bll;
using ServiceConfigManager.Core.Mapping;
using ServiceConfigManager.DataLayer;

namespace ServiceConfigManager.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        try
        {
            builder.Host.UseSerilog();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            builder.Services.ConfigureMassTransit(builder.Configuration);

            builder.Services.ConfigureApiServices(builder.Configuration);
            builder.Services.ConfigureBllServices();
            builder.Services.ConfigureDalServices();
            builder.Services.AddAutoMapper(typeof(RequestMapperProfile), typeof(ResponseMapperProfile));

            var app = builder.Build();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseMiddleware<IpBlockMiddelware>();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            Log.Information("Running app");

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.Information("App stopped");
            Log.CloseAndFlush();
        }
    }
}