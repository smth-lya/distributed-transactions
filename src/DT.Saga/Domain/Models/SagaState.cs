namespace DT.Saga.Domain.Models;

public class SagaState
{
    public required Guid CorrelationId { get; init; }
    public required Guid OrderId { get; init; }   // TODO: OrderId сильно конкретизирует сагу, нужно подумать как улучшить.
    public required string CurrentState { get; set; }                    // Current step: InventoryReserved, PaymentProcessed
    public required string SagaType { get; init; }                        // {"OrderProcessing"}. Нужно для разных типов саг.
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsCompleted { get; set; }
    public byte[] RowVersion { get; init; }                      // For optimistic lock (OCC)
    
    public ICollection<SagaEvent> Events { get; init; } = new List<SagaEvent>();
    public ICollection<SagaCommand> Commands { get; init; } = new List<SagaCommand>();
}