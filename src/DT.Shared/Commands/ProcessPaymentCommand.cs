using DT.Shared.Messaging;

namespace DT.Shared.Commands;

public record ProcessPaymentCommand(decimal Amount, string Currency, int UserId)
    : IMessage;