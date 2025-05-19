namespace DT.Inventories.Domain.Exceptions;

public class InsufficientStockException : Exception
{
    public InsufficientStockException(Guid productId, int available, int requested) 
        : base($"Insufficient stock for product {productId}. Available: {available}. Requested: {requested}")
    {
        ProductId = productId;
        Available = available;
        Requested = requested;
    }
    
    public Guid ProductId { get; }
    public int Available { get; }
    public int Requested { get; }
}