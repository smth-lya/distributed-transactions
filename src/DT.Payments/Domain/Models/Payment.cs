using DT.Payments.Domain.Enums;
using DT.Payments.Domain.Exceptions;
using DT.Shared.Enums;

namespace DT.Payments.Domain.Models;

public class Payment
{
    public Payment(Guid id, Guid orderId, decimal amount, PaymentStatus status = PaymentStatus.Pending)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        Id = id;
        OrderId = orderId;
        Amount = amount;
        Status = status;
        CreatedAt = DateTime.UtcNow;
    }
    
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public DateTime CreatedAt { get; init; }
    
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }

    public void MarkAsCompleted()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidPaymentStatusException(Status, PaymentStatus.Completed);
        
        Status = PaymentStatus.Completed;
    }

    public void MarkAsFailed()
    {
        Status = PaymentStatus.Failed;
    }
}