using DT.Common.Messaging;

namespace DT.Common.Events;

public record OrderSagaFailedEvent(string FailedStep, string Error)
    : Message;