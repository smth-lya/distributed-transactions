namespace DT.Orders.Application.DTOs;

public record OrderItemDto(Guid ProductId, int Quantity, decimal Price);