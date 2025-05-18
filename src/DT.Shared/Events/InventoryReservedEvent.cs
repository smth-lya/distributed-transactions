using DT.Shared.DTOs;
using DT.Shared.Messaging;

namespace DT.Shared.Events;

public record InventoryReservedEvent(List<InventoryItem> ReservedItems)
    : IMessage;