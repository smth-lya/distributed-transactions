namespace DT.Orders.API.Requests;

public record CreateOrderRequest(Guid CustomerId, List<OrderItemRequest> Items, string? PromoCode = null);