using DT.Inventories.Domain.Exceptions;

namespace DT.Inventories.Domain.Models;

public class Stock
{
    public Stock(Guid productId, int initialQuantity)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = initialQuantity;
        Reserved = 0;
    }
    
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; private set; }
    public int Reserved { get; private set; }
    
    
    public void Reserve(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        
        if (Quantity < quantity)
            throw new InsufficientStockException(ProductId, Quantity, quantity);
        
        Quantity -= quantity;
        Reserved += quantity;
    }

    public void Release(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(quantity, Reserved);
        
        Reserved -= quantity;
        Quantity += quantity;
    }
}