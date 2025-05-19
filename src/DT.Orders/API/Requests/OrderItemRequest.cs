namespace DT.Orders.API.Requests;

public record OrderItemRequest(Guid ProductId, int Quantity);