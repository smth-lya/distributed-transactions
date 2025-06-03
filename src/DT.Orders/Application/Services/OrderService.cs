using DT.Orders.Application.DTOs;
using DT.Orders.Domain.Contracts.Repositories;
using DT.Orders.Domain.Contracts.Services;
using DT.Orders.Domain.Enums;
using DT.Orders.Domain.Exceptions;
using DT.Orders.Domain.Models;
using DT.Shared.DTOs;
using DT.Shared.Events.Order;

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
        => await _orderRepository.GetByIdAsync(id);

    public async Task<Guid> CreateOrderAsync(OrderCreateDto createDto)
    {
        ArgumentNullException.ThrowIfNull(createDto, nameof(createDto));
        
        // Лучше сделать вытаскивать названия продуктов из отдельного сервиса или репозитория,
        // здесь "Unknown product name" - пока просто заглушка, которую лучше убрать или заменить
        var orderItems = createDto.Items.Select(i => 
            new OrderItem(
                i.ProductId, 
                "Unknown product name", // TODO: Сделать получение реального имени продукта
                i.Quantity, 
                i.Price
            )).ToList();
        
        var order = new Order(
            createDto.CustomerId,
            createDto.ShippingAddress,
            orderItems
            );

        await _orderRepository.AddAsync(order);
        
        return order.Id;
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, string reason)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new OrderNotFoundException(orderId);
        }
        
        order.ChangeStatus(newStatus, reason);
        await _orderRepository.UpdateAsync(order);
    }
}