using DT.Common.Messaging;

namespace DT.Common.Commands;

public record ProcessPaymentCommand(decimal Amount, string Currency, int UserId)
    : Message;