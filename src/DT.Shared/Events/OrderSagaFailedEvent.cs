using DT.Shared.Messaging;

namespace DT.Shared.Events;

public record OrderSagaFailedEvent(string FailedStep, string Error)
    : IMessage;