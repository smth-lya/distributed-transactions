using DT.Shared.Messaging;

namespace DT.Shared.Events.Payment;

public record PaymentCompletedEvent(decimal Amount)
    : IEvent;