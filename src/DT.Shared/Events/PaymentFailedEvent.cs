using DT.Shared.Messaging;

namespace DT.Shared.Events;

public record PaymentFailedEvent(string Reason, decimal AttemptedAmount)
    : IMessage;