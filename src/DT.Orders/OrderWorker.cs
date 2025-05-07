using System.Text.Json;
using DT.Common.Commands;
using DT.Common.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DT.Orders;

public class OrderWorker : BackgroundService
{
    private IConnection _connection;
    private IChannel _channel;

    private ILogger<OrderWorker> _logger;
    
    public OrderWorker(ILogger<OrderWorker> logger)
    {
        _logger = logger;
    }
    
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await CreateRabbitMq(cancellationToken);
        await ConfigureRabbitMq(cancellationToken);
        await SubscribeOnSagaCommands(cancellationToken);
        
        await base.StartAsync(cancellationToken);
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _channel.CloseAsync(cancellationToken);
        await _connection.CloseAsync(cancellationToken);
        
        await base.StopAsync(cancellationToken);
    }
    
    private async Task CreateRabbitMq(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        
        _connection = await factory.CreateConnectionAsync(cancellationToken: cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
    }
    private async Task ConfigureRabbitMq(CancellationToken cancellationToken = default)
    {
        await _channel.ExchangeDeclareAsync("saga.direct.cmd", ExchangeType.Direct, true, cancellationToken: cancellationToken);
        await _channel.ExchangeDeclareAsync("saga.fanout.evt", ExchangeType.Fanout, true, cancellationToken: cancellationToken);
        await _channel.ExchangeDeclareAsync("dlx.saga.cmd", ExchangeType.Fanout, true, cancellationToken: cancellationToken);
        await _channel.ExchangeDeclareAsync("dlx.saga.evt", ExchangeType.Fanout, true, cancellationToken: cancellationToken);
        
        await _channel.QueueDeclareAsync($"order.cmd.q", durable: true, exclusive: false, autoDelete: false,
            new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = "dlx.saga.cmd",
                ["x-max-priority"] = 10
            }, cancellationToken: cancellationToken);
        
        await _channel.QueueBindAsync($"order.cmd.q", "saga.direct.cmd", $"order", cancellationToken: cancellationToken);
        
        await _channel.QueueDeclareAsync("orchestrator.evt.q", durable: true, exclusive: false, autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-message-ttl"] = 86400000,
                ["x-dead-letter-exchange"] = "dlx.saga.evt",
            }, cancellationToken: cancellationToken);
        
        await _channel.QueueBindAsync("orchestrator.evt.q", "saga.fanout.evt", string.Empty, cancellationToken: cancellationToken);
        
        //DLQ
        await _channel.QueueDeclareAsync("dlx.saga.cmd.q",durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await _channel.QueueBindAsync("dlx.saga.cmd.q", "dlx.saga.cmd", string.Empty, cancellationToken: cancellationToken);
    }
    private async Task SubscribeOnSagaCommands(CancellationToken cancellationToken)
    {
        var tasks = new List<Task>()
        {
            SubscribeAsync<ApproveOrderCommand>("order.cmd.q", HandleApproveOrder, cancellationToken)
        };
        
        await Task.WhenAll(tasks);
    }

    private Task HandleApproveOrder(ApproveOrderCommand arg)
    {
        _logger.LogInformation(new string('E', 50));
        _logger.LogInformation(arg.CorrelationId.ToString());
        return Task.CompletedTask;
    }

    private async Task SubscribeAsync<T>(string queue, Func<T, Task> handler,
        CancellationToken cancellationToken = default) where T : Message
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var message = JsonSerializer.Deserialize<T>(ea.Body.Span);
                await handler(message!);
                
                await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
            }
            catch (Exception ex)
            {
                await _channel.BasicNackAsync(ea.DeliveryTag, false, false, cancellationToken);
            }
        };
        
        await _channel.BasicConsumeAsync(queue: queue, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
    }
    
    public async Task PublishAsync<T>(T message, string exchange = "", string routingKey = "", CancellationToken cancellationToken = default) where T : Message
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        var prop = new BasicProperties
        {
            ContentType = "application/json",
            Persistent = true,
            CorrelationId = message.CorrelationId.ToString(),
            Headers = new Dictionary<string, object?>
            {
                ["MessageType"] = typeof(T).Name,
            }
        };
        
        await _channel.BasicPublishAsync(exchange, routingKey, true, 
            prop, body, cancellationToken);
    }

}