using DT.Inventories.Domain.Models;

namespace DT.Inventories.Infrastructure.Database.Seed;

public class InventorySeedData
{
    private static readonly Guid ElectronicsCategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid ClothingCategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly Guid MainWarehouseId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid SecondaryWarehouseId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    private static readonly Guid LaptopProductId = Guid.Parse("55555555-5555-5555-5555-555555555555");
    private static readonly Guid TShirtProductId = Guid.Parse("66666666-6666-6666-6666-666666666666");

    private static List<Warehouse> GetWarehouses()
    {
        return
        [
            new Warehouse
            {
                Id = MainWarehouseId,
                Name = "Main Warehouse",
                Location = "New York, USA",
                IsActive = true
            },

            new Warehouse
            {
                Id = SecondaryWarehouseId,
                Name = "Secondary Warehouse",
                Location = "Los Angeles, USA",
                IsActive = true
            }
        ];
    }

    private static List<Product> GetProducts()
    {
        return
        [
            new Product
            {
                Id = LaptopProductId,
                SKU = "LT-1000",
                Name = "Premium Laptop",
                Description = "High performance laptop with 16GB RAM",
                CategoryId = ElectronicsCategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },

            new Product
            {
                Id = TShirtProductId,
                SKU = "TS-2000",
                Name = "Cotton T-Shirt",
                Description = "Comfortable 100% cotton t-shirt",
                CategoryId = ClothingCategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            }
        ];
    }

    private static List<InventoryItem> GetInventoryItems()
    {
        return
        [
            new InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = LaptopProductId,
                WarehouseId = MainWarehouseId,
                Quantity = 50,
                Reserved = 5,
                MinStockLevel = 10,
                LastStockedAt = DateTime.UtcNow.AddDays(-1)
            },

            new InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = TShirtProductId,
                WarehouseId = MainWarehouseId,
                Quantity = 200,
                Reserved = 20,
                MinStockLevel = 50,
                LastStockedAt = DateTime.UtcNow.AddDays(-3)
            },

            new InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = TShirtProductId,
                WarehouseId = SecondaryWarehouseId,
                Quantity = 100,
                Reserved = 10,
                MinStockLevel = 30,
                LastStockedAt = DateTime.UtcNow.AddDays(-5)
            }
        ];
    }

    public static async Task SeedAsync(InventoryDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (!context.Warehouses.Any())
        {
            context.Warehouses.AddRange(GetWarehouses());
        }

        if (!context.Products.Any())
        {
            context.Products.AddRange(GetProducts());
        }

        if (!context.InventoryItems.Any())
        {
            context.InventoryItems.AddRange(GetInventoryItems());
        }

        await context.SaveChangesAsync();
    }
    
    public static void Seed(InventoryDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Warehouses.Any())
        {
            context.Warehouses.AddRange(GetWarehouses());
        }

        if (!context.Products.Any())
        {
            context.Products.AddRange(GetProducts());
        }

        if (!context.InventoryItems.Any())
        {
            context.InventoryItems.AddRange(GetInventoryItems());
        }

        context.SaveChanges();
    }

}