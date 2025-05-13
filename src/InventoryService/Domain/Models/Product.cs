using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryService.Domain.Models;

public class Product
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    [Required]
    [ForeignKey("Warehouse")]
    public Guid WarehouseId { get; set; }
    
    [Required]
    public Warehouse Warehouse { get; set; }
    
}