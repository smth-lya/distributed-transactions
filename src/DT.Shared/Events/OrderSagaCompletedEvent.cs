using DT.Shared.Messaging;

namespace DT.Shared.Events;

public record OrderSagaCompletedEvent(DateTimeOffset CompletedAt) 
    : IMessage;