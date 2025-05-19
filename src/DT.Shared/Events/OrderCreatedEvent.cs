using DT.Shared.DTOs;
using DT.Shared.Messaging;

namespace DT.Shared.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    List<OrderItemShared> Items) 
    : IMessage;