using DT.Orders.Infrastructure.Messaging;
using DT.Shared.Messaging;

namespace DT.Orders.API.Extensions;

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