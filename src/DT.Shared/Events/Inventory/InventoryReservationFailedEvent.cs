using DT.Shared.DTOs;
using DT.Shared.Messaging;

namespace DT.Shared.Events.Inventory;

public record InventoryReservationFailedEvent(List<InventoryItemShared> Items, string Reason)
    : IMessage;