using DT.Payments.Application.DTOs;
using DT.Payments.Domain.Models;

namespace DT.Payments.Domain.Contracts.Services;

public interface IPaymentService
{
    Task<Payment> ProcessPaymentAsync(PaymentProcessDto processDto);
    
    Task RefundPaymentASync(Guid paymentId);
}