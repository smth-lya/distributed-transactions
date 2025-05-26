using DT.Inventories.Domain.Enums;

namespace DT.Inventories.Domain.Models;

public class InventoryMovement
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public int QuantityDelta { get; set; }
    public MovementType MovementType { get; set; }
    public Guid? RelatedOrderId { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; }
    
    public Product Product { get; set; }
    public Warehouse Warehouse { get; set; }
}