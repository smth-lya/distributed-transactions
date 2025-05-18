using DT.Common.Messaging;

namespace DT.Common.Commands;

public record RefundPaymentCommand(string TransactionId)
    : Message;