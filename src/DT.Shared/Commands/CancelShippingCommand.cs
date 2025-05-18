namespace DT.Shared.Commands;

public record CancelShippingCommand(
    Guid ShippingId,
    string Reason);