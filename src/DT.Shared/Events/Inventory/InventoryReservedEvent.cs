using DT.Shared.DTOs;
using DT.Shared.Messaging;

namespace DT.Shared.Events.Inventory;

public record InventoryReservedEvent(List<InventoryItemShared> ReservedItems)
    : IMessage;