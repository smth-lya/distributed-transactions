using DT.Shared.Messaging;

namespace DT.Shared.Commands.Order;

public record OrderRejectCommand(Guid OrderId, string Reason)
    : IMessage;