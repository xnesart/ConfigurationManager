using MassTransit;
using Messaging.Shared;

namespace ServiceConfigManager.Api.Extensions;

public static class ConfigurationForMassTransit
{
    public static IServiceCollection ConfigureMassTransit(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMq:Host"]);

                cfg.Message<ConfigurationMessage>(m => { m.SetEntityName("configurations-exchange"); });

                cfg.Publish<ConfigurationMessage>(p =>
                {
                    p.ExchangeType = "fanout";
                    p.Durable = true;
                });

                cfg.ReceiveEndpoint("configurations-queue", e =>
                {
                    e.Durable = true;
                    e.AutoDelete = true;
                    e.Exclusive = true;
                    // Настройка TTL и сохранения сообщений
                    e.SetQueueArgument("x-message-ttl", 10 * 60 * 1000); // 10 минут в миллисекундах
                });
            });
        });

        return services;
    }
}