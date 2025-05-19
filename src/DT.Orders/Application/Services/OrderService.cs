using DT.Orders.Application.DTOs;
using DT.Orders.Domain.Contracts.Repositories;
using DT.Orders.Domain.Contracts.Services;
using DT.Orders.Domain.Exceptions;
using DT.Orders.Domain.Models;

namespace DT.Orders.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderService> _logger;
    
    public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await _orderRepository.GetByIdAsync(id);
    }

    public async Task<Order> CreateOrderAsync(OrderCreateDto createDto)
    {
        var orderId = Guid.NewGuid();
        
        var order = new Order(
            orderId,
            createDto.CustomerId,
            createDto.Items.Select(i => new OrderItem(orderId, i.ProductId, i.Quantity, i.Price))
            );
        
        await _orderRepository.AddAsync(order);
        return order;
    }

    public async Task ReserveOrderAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId)
                    ?? throw new OrderNotFoundException(orderId);
        
        order.MarkAsReserved();
        await _orderRepository.UpdateAsync(order);
    }

    public async Task CancelOrderAsync(Guid orderId)
    {
        throw new NotImplementedException();
    }
}