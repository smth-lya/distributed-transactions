using DT.Common.DTOs;
using DT.Common.Messaging;

namespace DT.Common.Events;

public record InventoryReservedEvent(Guid ReservationId, List<InventoryItem> ReservedItems)
    : Message;