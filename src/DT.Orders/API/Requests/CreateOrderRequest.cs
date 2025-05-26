using DT.Orders.Domain.Models;

namespace DT.Orders.API.Requests;

public record CreateOrderRequest(Guid CustomerId, Address ShippingAddress, List<OrderItemRequest> Items, string? PromoCode = null);