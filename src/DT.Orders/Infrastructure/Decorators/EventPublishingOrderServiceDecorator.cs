using DT.Orders.Application.DTOs;
using DT.Orders.Domain.Contracts.Services;
using DT.Orders.Domain.Enums;
using DT.Orders.Domain.Models;
using DT.Shared.DTOs;
using DT.Shared.Events.Order;
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
    
    public async Task<Guid> CreateOrderAsync(OrderCreateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        
        var orderId = await _inner.CreateOrderAsync(dto);
        var publishingEvent = new OrderCreatedEvent(
            orderId,
            dto.Items
                .Select(i => new OrderItemShared(i.ProductId, i.Quantity, i.Price))
                .ToList()
        );

        try
        {
            await _publisher.PublishAsync(
                publishingEvent,
                "saga.orchestration.events",
                string.Empty,
                Guid.NewGuid()
            );
            
            _logger.LogInformation("Published OrderCreatedEvent for OrderId: {OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish OrderCreatedEvent for OrderId: {OrderId}", orderId);
            // TODO: Здесь реализовать retry-политику или оповещение (через Dead Letter Queue)
            throw;
        }
        
        return orderId;
    }
    
    public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, string reason)
        => await _inner.UpdateOrderStatusAsync(orderId, newStatus, reason);
}


