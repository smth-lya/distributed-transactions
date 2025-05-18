using System.Text.Json;
using DT.Shared.Interfaces;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DT.Shared.Messaging;

public abstract class RabbitMqBrokerBase : IMessagePublisher, IMessageSubscriber, IHostedService, IDisposable, IAsyncDisposable
{
    private IConnection _connection = null!;
    private IChannel _channel = null!;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await CreateAsync(cancellationToken);
        await ConfigureTopologyAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _channel.CloseAsync(cancellationToken);
        await _connection.CloseAsync(cancellationToken);
    }
    
        public async Task PublishAsync<T>(
        T message, 
        string exchange, 
        string routingKey, 
        string? correlationId, 
        CancellationToken cancellationToken = default) where T : IMessage
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        var prop = new BasicProperties
        {
            ContentType = "application/json",
            Persistent = true,
            CorrelationId = correlationId ?? Guid.NewGuid().ToString(),
            Headers = new Dictionary<string, object?>
            {
                ["MessageType"] = typeof(T).Name,
            }
        };
        
        await _channel.BasicPublishAsync(exchange, routingKey, true, 
            prop, body, cancellationToken);
    }
    
    public async Task SubscribeAsync<T>(
        string queue, 
        IConsumer<T> handler,
        CancellationToken cancellationToken = default) where T : IMessage
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                if (ea.BasicProperties.Headers != null &&
                    ea.BasicProperties.Headers.TryGetValue("MessageType", out var messageType) &&
                    messageType as string != typeof(T).Name)
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false, cancellationToken);
                    return;
                }
                
                var correlationId = ea.BasicProperties.CorrelationId ?? Guid.NewGuid().ToString();
                
                var message = JsonSerializer.Deserialize<T>(ea.Body.Span);
                if (message == null)
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false, cancellationToken);
                    return;
                }

                var context = new ConsumeContext<T>()
                {
                    Message = message,
                    CorrelationId = correlationId,
                    Publisher = this
                };
                
                await handler.Consume(context);
                
                await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
            }
            catch (Exception ex)
            {
                await _channel.BasicNackAsync(ea.DeliveryTag, false, false, cancellationToken);
            }
        };
        
        await _channel.BasicConsumeAsync(queue: queue, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
    }
    
    protected abstract Task ConfigureTopologyAsync(CancellationToken cancellationToken = default);
   
    protected virtual async Task CreateAsync(CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        
        _connection = await factory.CreateConnectionAsync(cancellationToken: cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
    }

    protected virtual async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await CreateAsync(cancellationToken);
        await DeclareCommonInfrastructureAsync(cancellationToken);
        await ConfigureTopologyAsync(cancellationToken);
    }

    protected virtual Task DeclareQueueAsync(string queueName)
        => _channel.QueueDeclareAsync(queueName, true, false, false, null);
    
    protected virtual Task DeclareExchangeAsync(string exchangeName, string exchangeType)
        => _channel.ExchangeDeclareAsync(exchangeName, exchangeType, true, false, null);
    
    protected virtual Task DeclareQueueBindAsync(string queueName, string exchangeName, string routingKey)
        => _channel.QueueBindAsync(queueName, exchangeName, routingKey);

    private async Task DeclareCommonInfrastructureAsync(CancellationToken cancellationToken)
    {
        await _channel.ExchangeDeclareAsync(
            exchange: "global.dlx",
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false, cancellationToken: cancellationToken);
    }

    
    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
            await _connection.DisposeAsync();

        if (_channel != null)
            await _channel.DisposeAsync();
    }
}