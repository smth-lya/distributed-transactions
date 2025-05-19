namespace DT.Payments.Domain.Exceptions;

public class PaymentFailedException : Exception
{
    public PaymentFailedException(Guid paymentId, string reason)
        : base($"Payment {paymentId} failed: {reason}")
    {
        PaymentId = paymentId;
        Reason = reason;
    }
    
    public Guid PaymentId { get; }
    public string Reason { get; }
}