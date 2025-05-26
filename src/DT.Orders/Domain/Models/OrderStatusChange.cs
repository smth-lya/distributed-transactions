using DT.Orders.Domain.Enums;

namespace DT.Orders.Domain.Models;

public class OrderStatusChange
{
    public OrderStatusChange(Guid orderId, OrderStatus status, string changeReason)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        Status = status;
        ChangeReason = changeReason;
        ChangedAt = DateTime.UtcNow;
    }
    
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public OrderStatus Status { get; private set; }
    public string ChangeReason { get; private set; }
    public DateTime ChangedAt { get; private set; }
}