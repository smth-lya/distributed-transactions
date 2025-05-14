namespace DT.Saga.States;

public class OrderState : ISagaState
{
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public SagaStatus Status { get; set; } = SagaStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Dictionary<string, object> ContextData { get; set; } = new();

    public int UserId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }

    public bool InventoryReserved { get; set; }
    public bool PaymentProcessed { get; set; }
    public bool ShippingScheduled { get; set; }

    public void SetData<T>(string key, T value) => ContextData[key] = value;
    public T? GetData<T>(string key) => ContextData.TryGetValue(key, out var val) ? (T)val : default;
}

public record OrderItem(int ProductId, int Quantity, decimal Price);
