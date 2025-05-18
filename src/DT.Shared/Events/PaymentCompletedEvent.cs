using DT.Common.Messaging;

namespace DT.Common.Events;

public record PaymentCompletedEvent(decimal Amount)
    : IMessage;