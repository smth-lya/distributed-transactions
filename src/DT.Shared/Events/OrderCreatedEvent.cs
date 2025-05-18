using DT.Common.Messaging;

namespace DT.Common.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    decimal TotalPrice
) : IMessage;