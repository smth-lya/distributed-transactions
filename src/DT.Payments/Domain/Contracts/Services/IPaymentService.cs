namespace DT.Payments.Domain.Contracts.Services;

public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(Guid orderId, decimal amount);
    
    Task<bool> RefundPaymentAsync(Guid paymentId);
}