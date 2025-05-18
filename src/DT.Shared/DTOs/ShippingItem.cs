namespace DT.Shared.DTOs;

public record ShippingItem(
    int ProductId,
    int Quantity,
    decimal WeightKg);