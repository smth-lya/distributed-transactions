using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DT.Inventories.Domain.Models;

public class Product
{
    public Guid Id { get; set; }
    public string SKU { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid CategoryId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public ICollection<InventoryItem> InventoryItems { get; set; }
    public ICollection<Reservation> Reservations { get; set; }
    public ICollection<InventoryMovement> Movements { get; set; }
}