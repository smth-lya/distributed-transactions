namespace DT.Saga.Domain.Exceptions;

public class SagaAlreadyExistsException : Exception
{
    public SagaAlreadyExistsException(Guid correlationId) : base($"Saga with ID {correlationId} already exists")
    {
        CorrelationId = correlationId;
    }
 
    public Guid CorrelationId { get; }
}