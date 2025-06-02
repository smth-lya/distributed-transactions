using DT.Shared.Messaging;
using RabbitMQ.Client;

namespace DT.Orders.Outbox.Infrastructure.Messaging;

public class RabbitMqBroker : RabbitMqBrokerBase
{
    public RabbitMqBroker(IOutboxPublisher outboxPublisher) : base(outboxPublisher)
    { }

    protected override async Task ConfigureTopologyAsync(CancellationToken cancellationToken = default)
    {
        await DeclareExchangeAsync("saga.orchestration.commands", ExchangeType.Direct);
        await DeclareExchangeAsync("saga.orchestration.events", ExchangeType.Fanout);
        
        var services = new[] { "inventory" };
        foreach (var service in services)
        {
            var exchangeName = $"{service}.saga.orchestration.commands";
        
            await DeclareExchangeAsync(exchangeName, ExchangeType.Fanout);
            await DeclareExchangeBindAsync(exchangeName, "saga.orchestration.commands", service);
        }
    }
}