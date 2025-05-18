using DT.Common.Messaging;

namespace DT.Common.Events;

public record OrderSagaCompletedEvent(DateTimeOffset CompletedAt) 
    : Message;