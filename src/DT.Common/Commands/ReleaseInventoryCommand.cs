using DT.Common.Messaging;

namespace DT.Common.Commands;

public record ReleaseInventoryCommand(Guid ReservationId)
    : Message;