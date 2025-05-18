using DT.Common.Messaging;

namespace DT.Common.Events;

public record PaymentFailedEvent(string Reason, decimal AttemptedAmount)
    : IMessage;