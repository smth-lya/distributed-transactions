namespace DT.Orders.Application.DTOs;

public record OrderCreateDto(Guid CustomerId, IReadOnlyCollection<OrderItemDto> Items, string? DiscountCode = null);