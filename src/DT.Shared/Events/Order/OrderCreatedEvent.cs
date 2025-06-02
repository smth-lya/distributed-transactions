using DT.Shared.DTOs;
using DT.Shared.Messaging;

namespace DT.Shared.Events.Order;

public record OrderCreatedEvent(
    Guid OrderId,
    List<OrderItemShared> Items) 
    : IEvent;