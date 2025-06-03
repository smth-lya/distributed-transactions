using DT.Shared.Messaging;
using RabbitMQ.Client;

namespace DT.Saga.Outbox.Infrastructure.Messaging;

public class RabbitMqBroker : RabbitMqBrokerBase
{
    protected override async Task ConfigureTopologyAsync(CancellationToken cancellationToken = default)
    {
        await DeclareExchangeAsync("saga.orchestration.commands", ExchangeType.Direct);
                
        var services = new[] { "order", "payment", "inventory" };
        foreach (var service in services)
        {
            var exchangeName = $"{service}.saga.orchestration.commands";
        
            await DeclareExchangeAsync(exchangeName, ExchangeType.Fanout);
            await DeclareExchangeBindAsync(exchangeName, "saga.orchestration.commands", service);
        }
    }
}