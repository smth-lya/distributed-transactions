using DT.Saga.Core;
using DT.Saga.States;

namespace DT.Saga.Steps;

public class ReleaseInventoryStep : ISagaStep<OrderState>
{
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
