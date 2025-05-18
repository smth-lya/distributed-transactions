using DT.Shared.Messaging;
using DT.Saga.Messaging;

namespace DT.Saga.Extensions;

public static class RabbitMqBrokerExtensions
{
    public static void AddRabbitMq(this IServiceCollection services)
    {
        services.AddSingleton<RabbitMqBroker>();
        services.AddSingleton<IMessagePublisher>(sp => sp.GetRequiredService<RabbitMqBroker>());
        services.AddSingleton<IMessageSubscriber>(sp => sp.GetRequiredService<RabbitMqBroker>());
        services.AddHostedService(sp => sp.GetRequiredService<RabbitMqBroker>());
    }
}