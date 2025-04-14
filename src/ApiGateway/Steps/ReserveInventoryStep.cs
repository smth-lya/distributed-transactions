using ApiGateway.States;

namespace ApiGateway.Steps;

public class ReserveInventoryStep : ISagaStep<OrderState>
{
    private readonly IMessageBus _bus;

    public ReserveInventoryStep(IMessageBus bus)
    {
        _bus = bus;
    }

    public async Task CompensateAsync(OrderState state, ISagaContext context)
    {
        var request = new ReserveInventoryCommand();

        await _bus.PublishAsync(request);

        var responce = await context.WaitForEventAsync<InventoryReservedEvent>(
            TimeSpan.FromSeconds(30);
    }

    public Task ExecuteAsync(OrderState state, ISagaContext context)
    {
        throw new NotImplementedException();
    }
}

public c