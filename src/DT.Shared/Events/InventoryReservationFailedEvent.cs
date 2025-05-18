using DT.Shared.Messaging;

namespace DT.Shared.Events;

public record InventoryReservationFailedEvent(string Reason)
    : IMessage;