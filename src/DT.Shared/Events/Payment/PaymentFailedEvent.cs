using DT.Shared.Messaging;

namespace DT.Shared.Events.Payment;

public record PaymentFailedEvent(string Reason, decimal AttemptedAmount)
    : IEvent;