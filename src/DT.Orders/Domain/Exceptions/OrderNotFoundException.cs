namespace DT.Orders.Domain.Exceptions;

public class OrderNotFoundException : Exception
{
    public OrderNotFoundException(Guid orderId) : base($"Order with ID {orderId} not found")
    {
        OrderId = orderId;
    }
 
    public Guid OrderId { get; }
}