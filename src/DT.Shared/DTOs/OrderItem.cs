namespace DT.Common.DTOs;

public record OrderItem(
    int ProductId,
    string ProductName,
    decimal Price,
    int Quantity);