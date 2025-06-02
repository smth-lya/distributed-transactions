using DT.Shared.Messaging;

namespace DT.Shared.Commands.Payment;

public record PaymentRefundCommand(Guid OrderId)
    : ICommand;