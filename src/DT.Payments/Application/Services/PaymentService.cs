using DT.Payments.Application.DTOs;
using DT.Payments.Domain.Contracts.Repositories;
using DT.Payments.Domain.Contracts.Services;
using DT.Payments.Domain.Models;
using DT.Payments.Infrastructure.Database;

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
    
    public async Task<Payment> ProcessPaymentAsync(PaymentProcessDto processDto)
    {
        var payment = new Payment(Guid.NewGuid(), processDto.OrderId, processDto.Amount);

        try
        {
            // await _paymentGateway.ProcessPaymentAsync
            payment.MarkAsCompleted();

            // TODO: Добавить catch PaymentGatewayException с failed возвратом, чтобы был паблишинг PaymentFailedEvent
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await _paymentRepository.AddAsync(payment);
        }
        
        return payment;
    }

    public async Task RefundPaymentASync(Guid paymentId)
    {
        throw new NotImplementedException();
    }
}