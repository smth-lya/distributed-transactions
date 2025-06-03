using DT.Inventories.Infrastructure.Database;
using DT.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace DT.Inventories.Infrastructure.Messaging;

public class TransactionalOutboxDecorator : IMessagePublisher
{
    private readonly IMessagePublisher _inner;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TransactionalOutboxDecorator> _logger;
    
    public TransactionalOutboxDecorator(IMessagePublisher inner, IServiceScopeFactory scopeFactory, ILogger<TransactionalOutboxDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task PublishAsync<T>(
        T message,
        string exchange,
        string routingKey,
        Guid? correlationId,
        CancellationToken cancellationToken = default) where T : IMessage
    {
        Console.WriteLine(new string('T', 50));
        Console.WriteLine(new string('T', 50));
        await SaveToOutboxAsync(message, exchange, routingKey, correlationId, cancellationToken);
    }

    private async Task SaveToOutboxAsync<T>(
        T message, 
        string exchange, 
        string routingKey, 
        Guid? correlationId, CancellationToken cancellationToken) where T : IMessage
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
            
            var outboxMessage = OutboxMessage.Create(
                message,
                exchange,
                routingKey,
                correlationId ?? Guid.NewGuid());
            
            await dbContext.Set<OutboxMessage>().AddAsync(outboxMessage, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            // Не вызываем SaveChangesAsync - предполагается, что это сделает вызывающий код
            // в рамках общей транзакции бизнес-операции

            //await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Message added to outbox in transaction");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save message to outbox. Correlation: {CorrelationId}", correlationId);
            throw;
        }
    }
}