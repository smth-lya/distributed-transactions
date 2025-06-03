using DT.Shared.Messaging;
using RabbitMQ.Client;

namespace DT.Orders.Infrastructure.Messaging;

public class RabbitMqBroker : RabbitMqBrokerBase
{
    protected override async Task ConfigureTopologyAsync(CancellationToken cancellationToken = default)
    {
        await DeclareExchangeAsync("saga.orchestration.commands", ExchangeType.Direct);
        
        var services = new[] { "order" };
        foreach (var service in services)
        {
            var queueName = $"{service}.saga.orchestration.commands";
        
            await DeclareQueueAsync(queueName);
            await DeclareQueueBindAsync(queueName, "saga.orchestration.commands", service);
        }
    }
}