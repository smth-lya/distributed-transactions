namespace DT.Common.DTOs;

public record ShippingItem(
    int ProductId,
    int Quantity,
    decimal WeightKg);