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
        _logger.LogInformation("Процессор Outbox запущен.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();

                _logger.LogDebug("Получение необработанных сообщений из Outbox...");

                var messages = await context.OutboxMessages
                    .Where(m => m.ProcessedAt == null)
                    .OrderBy(m => m.CreatedAt)
                    .Take(100)
                    .ToListAsync(cancellationToken: stoppingToken);

                _logger.LogInformation("Найдено {MessageCount} необработанных сообщений.", messages.Count);

                foreach (var message in messages)
                {
                    try
                    {
                        _logger.LogDebug("Обработка сообщения {MessageId} типа {MessageType}.", message.Id, message.MessageType);

                        var messageType = AppDomain.CurrentDomain
                            .GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .FirstOrDefault(t => t.FullName == message.MessageType);

                        if (messageType == null)
                        {
                            _logger.LogWarning("Неизвестный тип сообщения: {MessageType} (ID: {MessageId})", message.MessageType, message.Id);
                            continue;
                        }

                        var payload = JsonSerializer.Deserialize(message.Payload, messageType);

                        if (payload is not IMessage typedMessage)
                        {
                            _logger.LogWarning("Не удалось десериализовать сообщение {MessageId} или результат не реализует IMessage.", message.Id);
                            continue;
                        }

                        _logger.LogDebug("Публикация сообщения {MessageId} в обменник '{Exchange}' с маршрутом '{RoutingKey}'.",
                            message.Id, message.Exchange, message.RoutingKey);

                        await publisher.PublishAsync(
                            typedMessage,
                            message.Exchange,
                            message.RoutingKey,
                            message.CorrelationId,
                            stoppingToken);

                        message.MarkAsProcessed();
                        await context.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Сообщение {MessageId} успешно обработано.", message.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при обработке сообщения {MessageId}.", message.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при выполнении цикла обработки Outbox.");
            }

            _logger.LogDebug("Ожидание 5 секунд перед следующей итерацией...");
            await Task.Delay(5000, stoppingToken);
        }

        _logger.LogInformation("Процессор Outbox остановлен.");
    }
}
