namespace DT.Common.Commands;

public record CancelShippingCommand(
    Guid ShippingId,
    string Reason);