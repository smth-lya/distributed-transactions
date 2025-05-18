using DT.Shared.Messaging;

namespace DT.Shared.Commands;

public record RefundPaymentCommand(string TransactionId)
    : IMessage;