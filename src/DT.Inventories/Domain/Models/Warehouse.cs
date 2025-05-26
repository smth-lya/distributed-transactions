namespace DT.Inventories.Domain.Models;

public class Warehouse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public bool IsActive { get; set; } = true;
 
    public ICollection<InventoryItem> InventoryItems { get; set; }
    public ICollection<InventoryMovement> Movements { get; set; }
}