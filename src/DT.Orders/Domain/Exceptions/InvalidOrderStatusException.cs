using DT.Orders.Domain.Enums;

namespace DT.Orders.Domain.Exceptions;

public class InvalidOrderStatusException : Exception
{
    public InvalidOrderStatusException(OrderStatus currentStatus, OrderStatus? requiredStatus = null)
        : base(requiredStatus.HasValue
            ? $"Invalid order status: {currentStatus}. Required status: {requiredStatus}"
            : $"Invalid order status: {currentStatus}")
    {
        CurrentStatus = currentStatus;
        RequiredStatus = requiredStatus;
    }

    public OrderStatus CurrentStatus { get; }
    public OrderStatus? RequiredStatus { get; }
}