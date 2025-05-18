using DT.Shared.Messaging;

namespace DT.Shared.Commands;

public record ReleaseInventoryCommand(Guid ReservationId)
    : IMessage;