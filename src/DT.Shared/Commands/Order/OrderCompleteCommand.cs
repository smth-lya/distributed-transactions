using DT.Shared.Messaging;

namespace DT.Shared.Commands.Order;

public record OrderCompleteCommand(Guid OrderId)
    : IMessage;