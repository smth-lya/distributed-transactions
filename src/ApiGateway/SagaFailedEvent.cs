public record SagaFailedEvent(
    Guid CorrelationId,
    Exception Exception,
    DateTimeOffset OccurredAt = default)
{
    public DateTimeOffset OccurredAt { get; } = OccurredAt == default
        ? DateTimeOffset.UtcNow
        : OccurredAt;
}
