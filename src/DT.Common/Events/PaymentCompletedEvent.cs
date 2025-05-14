using DT.Common.Messaging;

namespace DT.Common.Events;

public record PaymentCompletedEvent(string TransactionId, decimal Amount)
    : Message;