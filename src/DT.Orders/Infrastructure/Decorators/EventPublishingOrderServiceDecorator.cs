using DT.Orders.Application.DTOs;
using DT.Orders.Domain.Contracts.Services;
using DT.Orders.Domain.Models;
using DT.Shared.DTOs;
using DT.Shared.Events;
using DT.Shared.Messaging;

namespace DT.Orders.Infrastructure.Decorators;

public class EventPublishingOrderServiceDecorator : IOrderService
{
    private readonly IOrderService _inner;
    private readonly IMessagePublisher _publisher;
    
    private readonly ILogger<EventPublishingOrderServiceDecorator> _logger;

    public EventPublishingOrderServiceDecorator(IOrderService inner, IMessagePublisher publisher, ILogger<EventPublishingOrderServiceDecorator> logger)
    {
        _inner = inner;
        _publisher = publisher;
        _logger = logger;
    }

    public Task<Order?> GetOrderByIdAsync(Guid id)
        => _inner.GetOrderByIdAsync(id);
    public async Task<Order> CreateOrderAsync(OrderCreateDto dto)
    {
        var order = await _inner.CreateOrderAsync(dto);
        await _publisher.PublishAsync(
            new OrderCreatedEvent(
                order.Id, 
                dto.Items.Select(i => new OrderItemShared(i.ProductId, i.Quantity, i.Price)).ToList()
                ),
            "saga.orchestration.events",
            string.Empty,
            Guid.NewGuid().ToString());
        return order;
    }

    public Task ReserveOrderAsync(Guid orderId)
        => _inner.ReserveOrderAsync(orderId);

    public Task CancelOrderAsync(Guid orderId)
        => _inner.CancelOrderAsync(orderId);
}