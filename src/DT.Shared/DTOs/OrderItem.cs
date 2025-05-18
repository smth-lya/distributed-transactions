namespace DT.Shared.DTOs;

public record OrderItem(
    int ProductId,
    string ProductName,
    decimal Price,
    int Quantity);