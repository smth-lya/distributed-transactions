using DT.Shared.Messaging;

namespace DT.Shared.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    decimal TotalPrice
) : IMessage;