using InventoryService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost:5555;Username=postgres;Password=1;Database=inventory_db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().ToTable("users");
        modelBuilder.Entity<Warehouse>().ToTable("warehouses");
    } 
}
