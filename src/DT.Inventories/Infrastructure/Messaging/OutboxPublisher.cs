using DT.Inventories.Infrastructure.Database;
using DT.Shared.Messaging;

namespace DT.Inventories.Infrastructure.Messaging;

public class OutboxPublisher : IOutboxPublisher
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxPublisher> _logger;

    public OutboxPublisher(IServiceScopeFactory scopeFactory, ILogger<OutboxPublisher> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task PublishAsync<T>(
        T message,
        string exchange,
        string routingKey,
        Guid? correlationId,
        CancellationToken cancellationToken = default) where T : IMessage
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
            
            _logger.LogInformation("Message {MessageId} saved to outbox", outboxMessage.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save message to outbox");
            throw;
        }
    }
}