using ApiGateway.States;
using System.Data;
using ApiGateway.Core;

namespace ApiGateway.Steps;

public class ProcessPaymentStep : ISagaStep<OrderState>
{
    private readonly IMessageBus _bus;

    public ProcessPaymentStep(IMessageBus bus) => _bus = bus;

    public string Name { get; }

    public async Task ExecuteAsync(OrderState state, ISagaContext context)
    {
        throw new NotImplementedException();
    }

    public async Task CompensateAsync(OrderState state, ISagaContext context)
    {
        throw new NotImplementedException();
    }
}
