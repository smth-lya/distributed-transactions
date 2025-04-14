using ApiGateway.States;
using System.Data;
using ApiGateway.Core;

namespace ApiGateway.Steps;

public class ProcessPaymentStep : ISagaStep<OrderState>
{
    private readonly IMessageBus _bus;

    public ProcessPaymentStep(IMessageBus bus) => _bus = bus;

    public async Task ExecuteAsync(OrderState state, ISagaContext context)
    {
        var paymentRequest = new ProcessPaymentCommand(
            state.CorrelationId,
            state.UserId,
            state.TotalAmount,
            Currency.USD
        );

        await _bus.PublishAsync(paymentRequest);
        var result = await context.WaitForEventAsync<PaymentCompletedEvent>(
            TimeSpan.FromSeconds(60));

        if (result.Status != PaymentStatus.Approved)
            throw new SagaException($"Payment declined: {result.Reason}");

        state.PaymentProcessed = true;
    }

    public async Task CompensateAsync(OrderState state, ISagaContext context)
    {
        if (!state.PaymentProcessed) return;

        await _bus.PublishAsync(new RefundPaymentCommand(
            state.CorrelationId,
            state.TotalAmount
        ));
    }
}
