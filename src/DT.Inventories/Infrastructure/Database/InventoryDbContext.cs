using DT.Inventories.Domain.Models;
using DT.Inventories.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DT.Inventories.Infrastructure.Database;

public sealed class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    { }

    public DbSet<Stock> Stocks => Set<Stock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new StockConfiguration());
    } 
}
