using DT.Shared.Messaging;
using RabbitMQ.Client;

namespace DT.Inventories.Outbox.Infrastructure.Messaging;

public class RabbitMqBroker : RabbitMqBrokerBase
{
    protected override async Task ConfigureTopologyAsync(CancellationToken cancellationToken = default)
    {
        await DeclareExchangeAsync("saga.orchestration.events", ExchangeType.Fanout);
    }
}