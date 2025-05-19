using DT.Orders.Domain.Enums;
using DT.Orders.Domain.Exceptions;

namespace DT.Orders.Domain.Models;

public class Order
{
    private readonly List<OrderItem> _items = new();

    public Order(Guid id, Guid customerId, IEnumerable<OrderItem> items, OrderStatus status = OrderStatus.Pending)
    {
        Id = id;
        CustomerId = customerId;
        Status = status;
        _items.AddRange(items);
    }
    
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    
    public OrderStatus Status { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public void MarkAsReserved()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOrderStatusException(Status, OrderStatus.Reserved);

        Status = OrderStatus.Reserved;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped)
            throw new InvalidOrderStatusException(Status, OrderStatus.Cancelled);
        
        Status = OrderStatus.Cancelled;
    }
}