using DT.Shared.Messaging;
using RabbitMQ.Client;

namespace DT.Payments.Outbox.Infrastructure.Messaging;

public class RabbitMqBroker : RabbitMqBrokerBase
{
    protected override async Task ConfigureTopologyAsync(CancellationToken cancellationToken = default)
    {
        await DeclareExchangeAsync("saga.orchestration.events", ExchangeType.Fanout);
    }
}