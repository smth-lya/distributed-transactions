namespace DT.Shared.DTOs;

public record ShippingItemShared(
    int ProductId,
    int Quantity,
    decimal WeightKg);