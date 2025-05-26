using DT.Payments.Domain.Contracts.Services;
using DT.Shared.Commands.Payment;
using DT.Shared.Events.Payment;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Payments.API.Consumers.Orchestration;

public class PaymentRefundConsumer : IConsumer<PaymentRefundCommand>, IHostedService
{
    private readonly IMessageSubscriber _subscriber;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<PaymentRefundConsumer> _logger;
    
    public PaymentRefundConsumer(IServiceScopeFactory scopeFactory, IMessageSubscriber subscriber, ILogger<PaymentRefundConsumer> logger)
    {
        _subscriber = subscriber;
        _serviceScopeFactory = scopeFactory;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<PaymentRefundCommand> context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPaymentService>();

        try
        {
            _logger.LogInformation(
                "Processing refund for Order: {OrderId}, Correlation: {CorrelationId}",
                context.Message.OrderId,
                context.CorrelationId
            );
            
            var refunded = await service.RefundPaymentAsync(context.Message.OrderId);

            if (!refunded)
            {
                _logger.LogError("Failed to refund payment {PaymentId}", context.Message.OrderId);
                return;
            }
        
            _logger.LogInformation("Refund payment processed successfully. CorrelationId: {CorrelationId}", context.CorrelationId);

            var paymentEvent = new PaymentRefundEvent(); 
            
            await context.PublishAsync(
                paymentEvent,
                "saga.orchestration.events",
                string.Empty
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error processing refund for Order: {OrderId}, Correlation: {CorrelationId}", 
                context.Message.OrderId,
                context.CorrelationId);
            
            throw;
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _subscriber.SubscribeAsync("payment.saga.orchestration.commands", this, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}