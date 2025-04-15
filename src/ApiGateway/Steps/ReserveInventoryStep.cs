using ApiGateway.Core;
using ApiGateway.States;

namespace ApiGateway.Steps;

public class ReserveInventoryStep : ISagaStep<OrderState>
{
    private readonly IMessageBus _bus;

    public ReserveInventoryStep(IMessageBus bus)
    {
        _bus = bus;
    }

    public string Name => throw new NotImplementedException();

    public Task CompensateAsync(OrderState state, ISagaContext context)
    {
        throw new NotImplementedException();
    }

    public Task ExecuteAsync(OrderState state, ISagaContext context)
    {
        throw new NotImplementedException();
    }
}