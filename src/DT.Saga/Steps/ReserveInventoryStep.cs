using DT.Shared.Commands;
using DT.Shared.Messaging;
using DT.Saga.Core;
using DT.Saga.States;

namespace DT.Saga.Steps;

public class ReserveInventoryStep : ISagaStep<OrderState>
{
    // private readonly IMessageBus _bus;
    //
    // public ReserveInventoryStep(IMessageBus bus)
    // {
    //     _bus = bus;
    // }

    public string Name => throw new NotImplementedException();

    public Task ExecuteAsync(OrderState state, ISagaContext context)
    {
        //var responce = await _bus.PublishAsync(new ReserveInventoryCommand(state.CorrelationId, state.Items));
        return Task.CompletedTask;
    }
    
    public Task CompensateAsync(OrderState state, ISagaContext context)
    {
        throw new NotImplementedException();
    }

}