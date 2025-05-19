using DT.Shared.Messaging;
using RabbitMQ.Client;

namespace DT.Payments.Infrastructure.Messaging;

public class RabbitMqBroker : RabbitMqBrokerBase
{
    protected override async Task ConfigureTopologyAsync(CancellationToken cancellationToken = default)
    {
        await DeclareExchangeAsync("saga.orchestration.commands", ExchangeType.Direct);
        await DeclareExchangeAsync("saga.orchestration.events", ExchangeType.Fanout);
        
        var services = new[] { "payment" };
        foreach (var service in services)
        {
            var queueName = $"{service}.saga.orchestration.commands";
        
            await DeclareQueueAsync(queueName);
            await DeclareQueueBindAsync(queueName, "saga.orchestration.commands", service);
        }

        await DeclareQueueAsync("saga.orchestrator.events");
        await DeclareQueueBindAsync("saga.orchestrator.events", "saga.orchestration.events", string.Empty);
        
        // Choreography

        await DeclareExchangeAsync("saga.choreography.events", ExchangeType.Fanout);
        
        await DeclareQueueAsync("payment.saga.choreography.events");
        await DeclareQueueBindAsync("payment.saga.choreography.events", "saga.choreography.events", string.Empty);
    }
}