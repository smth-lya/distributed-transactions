namespace DT.Shared.DTOs;

public record OrderItemShared(
    Guid ProductId,
    int Quantity,
    decimal Price);
