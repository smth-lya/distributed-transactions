using System.Diagnostics;
using System.Text;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DT.Shared.Messaging;

public class RabbitMqActivitySource
{
    public static readonly ActivitySource Source = new ("RabbitMQ.Client", "1.0.0");

    public static Activity? StartPublishActivity(
        string exchange, 
        string routingKey, 
        IBasicProperties properties)
    {
        var activity = Source.StartActivity("rabbitmq.publish", ActivityKind.Producer);

        if (activity == null)
            return null;
        
        activity.SetTag("messaging.system", "rabbitmq");
        activity.SetTag("messaging.destination", exchange);
        activity.SetTag("messaging.destination_kind", "exchange");
        activity.SetTag("messaging.rabbitmq.routing_key", routingKey);

        var context = new PropagationContext(activity.Context, Baggage.Current);
        Propagators.DefaultTextMapPropagator.Inject(context, properties.Headers,
            (headers, key, value) =>
            {
                if (headers != null) 
                    headers[key] = Encoding.UTF8.GetBytes(value);
            });
        
        return activity;
    }

    public static Activity? StartConsumeActivity(
        BasicDeliverEventArgs ea, 
        string queueName)
    {
        var parentContext = Propagators.DefaultTextMapPropagator.Extract(
            default,
            ea.BasicProperties.Headers,
            (headers, key) =>
            {
                if (headers != null && 
                    headers.TryGetValue(key, out var value) && value is byte[] bytes)
                    return [Encoding.UTF8.GetString(bytes)];
                
                return Array.Empty<string>();
            });
        
        var activity = Source.StartActivity(
            "rabbitmq.process", 
            ActivityKind.Consumer,
            parentContext.ActivityContext);
        
        if (activity == null)
            return null;
        
        activity.SetTag("messaging.system", "rabbitmq");
        activity.SetTag("messaging.destination", queueName);
        activity.SetTag("messaging.destination_kind", "queue");
        activity.SetTag("messaging.message_id", ea.BasicProperties.MessageId);
        activity.SetTag("messaging.correlation_id", ea.BasicProperties.CorrelationId);
        
        Baggage.Current = parentContext.Baggage;
        
        return activity;
    }
}