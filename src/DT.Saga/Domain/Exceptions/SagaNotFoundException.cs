namespace DT.Saga.Domain.Exceptions;

public class SagaNotFoundException : Exception
{
    public SagaNotFoundException(Guid correlationId) : base($"Saga with ID {correlationId} not found")
    {
        CorrelationId = correlationId;
    }
 
    public Guid CorrelationId { get; }
}