namespace DT.Orders.Domain.Enums;

public enum OrderStatus
{
    Created = 1,
    Pending = 2,
    Reserved = 3,
    Paid = 4,
    Shipped = 5,
    Delivered = 6,
    Cancelled = 7,
    Refunded = 8
}