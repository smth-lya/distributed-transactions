using Microsoft.AspNetCore.Mvc;

public class OrdersController : ControllerBase
{
    private readonly IOrderSagaService _saga;

    public OrdersController(IOrderSagaService saga)
    {
        _saga = saga;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder(CreateOrderCommand createCommand)
    {
        var result = await _saga.ExecuteOrderSagaAsync(createCommand);
        return result ? Ok("Order completed successfully") : StatusCode(500, "Order failed");
    }
}
