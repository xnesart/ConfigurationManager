using MassTransit;
using Messaging.Shared;
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

            builder.Services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq://localhost");

                    cfg.Message<ConfigurationMessage>(m => 
                    { 
                        m.SetEntityName("configurations-exchange");
                    });

                    cfg.Publish<ConfigurationMessage>(p => 
                    { 
                        p.ExchangeType = "fanout";
                        p.Durable = true;
                    });

                    cfg.ReceiveEndpoint("configurations-queue", e =>
                    {
                        // Настройка TTL и сохранения сообщений
                        e.SetQueueArgument("x-message-ttl", 10 * 60 * 1000); // 10 минут в миллисекундах
                    });
                });
            });

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
