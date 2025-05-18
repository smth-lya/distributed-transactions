namespace DT.Inventories.Domain.Models;

public class Warehouse
{
    public int Id { get; set; }
    public List<Product> Products { get; set; } 
}