using DT.Shared.Messaging;

namespace DT.Shared.Commands.Inventory;

public record InventoryCancelReservationCommand(Guid OrderId)
    : IMessage;