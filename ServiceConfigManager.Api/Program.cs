using MassTransit;
using Microsoft.AspNetCore.HttpOverrides;
using ServiceConfigManager.Api.Extensions;
using ServiceConfigManager.Bll;
using ServiceConfigManager.Core.Mapping;
using Serilog;
using ServiceConfigManager.Api.Configuration;
using ServiceConfigManager.DataLayer;

namespace ServiceConfigManager.Api;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq://localhost");

                    cfg.ConfigureEndpoints(context);
                });
            });

            builder.Services.ConfigureApiServices(builder.Configuration);
            builder.Services.ConfigureBllServices();
            builder.Services.ConfigureDalServices();
            builder.Services.AddAutoMapper(typeof(RequestMapperProfile), typeof(ResponseMapperProfile));

            builder.Host.UseSerilog();
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
            Log.Fatal(ex.Message);
        }
        finally
        {
            Log.Information("App stopped");
            Log.CloseAndFlush();
        }
    }
}