using DT.Common.Messaging;

namespace DT.Common.Events;

public record InventoryReservationFailedEvent(string Reason)
    : Message;