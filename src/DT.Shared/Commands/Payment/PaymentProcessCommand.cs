using DT.Shared.Messaging;

namespace DT.Shared.Commands.Payment;

public record PaymentProcessCommand(Guid OrderId, decimal Amount, string Currency)
    : ICommand;