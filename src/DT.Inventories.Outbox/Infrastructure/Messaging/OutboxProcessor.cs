using System.Text.Json;
using DT.Inventories.Outbox.Infrastructure.Database;
using DT.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace DT.Inventories.Outbox.Infrastructure.Messaging;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(IServiceScopeFactory serviceScopeFactory, ILogger<OutboxProcessor> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
                
                var messages = await context.OutboxMessages
                    .Where(m => m.ProcessedAt == null)
                    .OrderBy(m => m.CreatedAt)
                    .Take(100)
                    .ToListAsync(cancellationToken: stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        var messageType = Type.GetType(message.MessageType);
                        if (messageType == null)
                        {
                            _logger.LogWarning("Unknown message type: {MessageType}", message.MessageType);
                            continue;
                        }
                        
                        var payload = JsonSerializer.Deserialize(message.Payload, messageType);

                        if (payload is not IMessage typedMessage)
                        {
                            _logger.LogWarning("Deserialization failed for message {MessageId}", message.Id);
                            continue;
                        }
                        
                        await publisher.PublishAsync(
                            typedMessage,
                            message.Exchange,
                            message.RoutingKey,
                            message.CorrelationId,
                            stoppingToken);
                        
                        message.MarkAsProcessed();
                        await context.SaveChangesAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process message {MessageId}", message.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in outbox processing");
            }
            
            await Task.Delay(5000, stoppingToken);
        }
    }
}