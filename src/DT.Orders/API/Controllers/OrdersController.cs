using DT.Orders.API.Requests;
using DT.Orders.Application.DTOs;
using DT.Orders.Domain.Contracts.Services;
using DT.Shared.Events;
using Microsoft.AspNetCore.Mvc;

namespace DT.Orders.API.Controllers;

[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        _logger.LogInformation("Starting order worker /random");

        var price = 10m; // RPC запрос к микросервису отвечающего за цены.
        
        var dto = new OrderCreateDto(
            request.CustomerId,
            request.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity, price)).ToList(),
            request.PromoCode);

        var order = await _orderService.CreateOrderAsync(dto);
        
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) 
            return NotFound();
        
        return Ok(order);
    }
}