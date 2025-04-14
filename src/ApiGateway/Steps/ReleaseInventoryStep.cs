using ApiGateway.States;

namespace ApiGateway.Steps;

public class ReleaseInventoryStep : ISagaStep<OrderState>
{
    public Task CompensateAsync(OrderState state, ISagaContext context)
    {
        throw new NotImplementedException();
    }

    public Task ExecuteAsync(OrderState state, ISagaContext context)
    {
        throw new NotImplementedException();
    }
}
