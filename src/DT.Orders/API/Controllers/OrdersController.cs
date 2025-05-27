using DT.Orders.API.Requests;
using DT.Orders.Application.DTOs;
using DT.Orders.Domain.Contracts.Services;
using DT.Shared.Events;
using Microsoft.AspNetCore.Mvc;

namespace DT.Orders.API.Controllers;

[ApiController]
[Route("/")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        _logger.LogInformation("CreateOrder: Received new order creation request for CustomerId: {CustomerId}", request.CustomerId);

        // TODO: Подключить RPC/HTTP запрос к микросервису цен
        var price = 10m;
        
        var dto = new OrderCreateDto(
            request.CustomerId,
            request.ShippingAddress,
            request.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity, price)).ToList(),
            request.PromoCode);

        try
        {
            var orderId = await _orderService.CreateOrderAsync(dto);
            _logger.LogInformation("CreateOrder: Order successfully created with OrderId: {OrderId}", orderId);

            return CreatedAtAction(nameof(GetOrder), new { id = orderId }, new { orderId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateOrder: Error occurred while creating order for CustomerId: {CustomerId}", request.CustomerId);
            return StatusCode(500, "An error occurred while creating the order.");
        }
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        _logger.LogInformation("GetOrder: Attempting to retrieve order with ID: {OrderId}", id);

        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("GetOrder: Order with ID {OrderId} not found", id);
                return NotFound();
            }

            _logger.LogInformation("GetOrder: Successfully retrieved order with ID: {OrderId}", id);
            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetOrder: Error retrieving order with ID: {OrderId}", id);
            return StatusCode(500, "An error occurred while retrieving the order.");
        }
    }
}