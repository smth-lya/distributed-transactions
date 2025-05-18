using DT.Common.Messaging;
using RabbitMQ.Client;

namespace DT.Inventories.Messaging;

public class RabbitMqBroker : RabbitMqBrokerBase
{
    protected override async Task ConfigureTopologyAsync(CancellationToken cancellationToken = default)
    {
        await DeclareExchangeAsync("saga.orchestration.commands", ExchangeType.Direct);
        await DeclareExchangeAsync("saga.orchestration.events", ExchangeType.Fanout);
        
        var services = new[] { "inventory" };
        foreach (var service in services)
        {
            var queueName = $"{service}.saga.orchestration.commands";
        
            await DeclareQueueAsync(queueName);
            await DeclareQueueBindAsync(queueName, "saga.orchestration.commands", service);
        }

        await DeclareQueueAsync("saga.orchestrator.events");
        await DeclareQueueBindAsync("saga.orchestrator.events", "saga.orchestration.events", string.Empty);
    }
}