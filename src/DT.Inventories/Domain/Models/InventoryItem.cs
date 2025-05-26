namespace DT.Inventories.Domain.Models;

public class InventoryItem
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
    public int Reserved { get; set; }
    public int MinStockLevel { get; set; } = 5;
    public DateTime? LastStockedAt { get; set; }
    
    public Product Product { get; set; }
    public Warehouse Warehouse { get; set; }
}