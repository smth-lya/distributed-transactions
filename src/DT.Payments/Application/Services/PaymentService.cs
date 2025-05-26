using DT.Payments.Domain.Contracts.Repositories;
using DT.Payments.Domain.Contracts.Services;
using DT.Payments.Domain.Models;

namespace DT.Payments.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<PaymentService> _logger;
    
    public PaymentService(IPaymentRepository paymentRepository, ILogger<PaymentService> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }
    
    public async Task<bool> ProcessPaymentAsync(Guid orderId, decimal amount)
    {
        var payment = new Payment(Guid.NewGuid(), orderId, amount);

        try
        {
            // await _paymentGateway.ProcessPaymentAsync
            payment.MarkAsCompleted();

            // TODO: Добавить catch PaymentGatewayException с failed возвратом, чтобы был паблишинг PaymentFailedEvent
        }
        catch (Exception ex)
        {
            _logger.LogError("[Payment Service] [ProcessPaymentAsync] Error: {Error}", ex.Message);
            return false;
        }
        finally
        {
            await _paymentRepository.AddAsync(payment);
        }
        
        return true;
    }

    public Task<bool> RefundPaymentAsync(Guid paymentId)
    {
        return Task.FromResult(true);
    }
}