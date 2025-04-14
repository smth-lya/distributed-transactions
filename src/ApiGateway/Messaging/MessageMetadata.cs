namespace ApiGateway.Messaging;

public class MessageMetadata
{
    public Guid CorrelationId { get; set; }
    public string? TraceId { get; set; }
    public Dictionary<string, string>? Headers { get; set; }
}

