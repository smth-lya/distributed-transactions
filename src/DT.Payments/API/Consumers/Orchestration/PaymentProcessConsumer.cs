using DT.Payments.Domain.Contracts.Services;
using DT.Shared.Commands.Payment;
using DT.Shared.Events.Payment;
using DT.Shared.Interfaces;
using DT.Shared.Messaging;

namespace DT.Payments.API.Consumers.Orchestration;

public class PaymentProcessConsumer : IConsumer<PaymentProcessCommand>, IHostedService
{
    private readonly IMessageSubscriber _subscriber;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<PaymentProcessConsumer> _logger;
    
    public PaymentProcessConsumer(IServiceScopeFactory scopeFactory, IMessageSubscriber subscriber, ILogger<PaymentProcessConsumer> logger)
    {
        _subscriber = subscriber;
        _serviceScopeFactory = scopeFactory;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<PaymentProcessCommand> context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPaymentService>();

        try
        {
            _logger.LogInformation(
                "Processing payment for CorrelationId: {CorrelationId}, Amount: {Amount}", 
                context.CorrelationId,
                context.Message.Amount);
            
            var paymentSuccess = await service.ProcessPaymentAsync(
                context.Message.OrderId, 
                context.Message.Amount
            );

            if (!paymentSuccess)
            {
                _logger.LogWarning("Payment process failed. CorrelationId: {CorrelationId}", context.CorrelationId);
                
                await context.PublishAsync(
                    new PaymentFailedEvent("Payment error", AttemptedAmount: 1),
                    "saga.orchestration.events",
                    string.Empty
                );
            
                return;
            }
        
            _logger.LogInformation("Payment processed successfully. CorrelationId: {CorrelationId}", context.CorrelationId);

            var paymentEvent = new PaymentCompletedEvent(context.Message.Amount); 
            
            await context.PublishAsync(
                paymentEvent,
                "saga.orchestration.events",
                string.Empty
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reserving inventory. CorrelationId: {CorrelationId}", context.CorrelationId);
            
            await context.PublishAsync(
                new PaymentFailedEvent(ex.Message, AttemptedAmount: 1),
                "saga.orchestration.events",
                string.Empty
            );
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