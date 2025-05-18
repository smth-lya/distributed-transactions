using DT.Shared.Messaging;
using DT.Saga.Core;
using DT.Saga.States;

namespace DT.Saga.Steps;

public class ProcessPaymentStep : ISagaStep<OrderState>
{
    // private readonly IMessageBus _bus;
    //
    // public ProcessPaymentStep(IMessageBus bus) => _bus = bus;

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
