using System.ComponentModel.DataAnnotations;
using DT.Orders.Domain.Contracts;
using DT.Orders.Domain.Enums;
using DT.Orders.Domain.Exceptions;
using DT.Shared.Messaging;

namespace DT.Orders.Domain.Models;

public class Order
{
    private readonly List<OrderItem> _items = new();
    private readonly List<OrderStatusChange> _statusChanges = new();
    
    private Order() { }
    
    public Order(Guid customerId, Address shippingAddress, IEnumerable<OrderItem> items)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Created;
        CreatedAt = DateTime.UtcNow;
        
        _items.AddRange(items);
        CalculateTotalAmount();
    }
    
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Address ShippingAddress { get; private set; }
    public decimal TotalAmount { get; private set; }
    
    
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<OrderStatusChange> StatusChanges => _statusChanges.AsReadOnly();

    private void CalculateTotalAmount()
    {
        TotalAmount = _items.Sum(i => i.UnitPrice * i.Quantity);
    }

    public void ChangeStatus(OrderStatus newStatus, string reason = "Unknown")
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        AddStatusChange(newStatus, reason);
    }

    private void AddStatusChange(OrderStatus status, string reason)
    {
        _statusChanges.Add(new OrderStatusChange(Id, status, reason));
    }
}