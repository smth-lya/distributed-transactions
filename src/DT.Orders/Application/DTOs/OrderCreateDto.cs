using DT.Orders.Domain.Models;

namespace DT.Orders.Application.DTOs;

public record OrderCreateDto(Guid CustomerId, Address ShippingAddress, IReadOnlyCollection<OrderItemDto> Items, string? DiscountCode = null);