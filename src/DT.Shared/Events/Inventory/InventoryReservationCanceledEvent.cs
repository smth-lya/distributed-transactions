using DT.Shared.Messaging;

namespace DT.Shared.Events.Inventory;

public record InventoryReservationCanceledEvent(string Reason)
    : IMessage;