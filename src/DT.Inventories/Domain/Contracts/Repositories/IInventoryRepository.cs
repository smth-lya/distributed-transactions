using DT.Inventories.Domain.Models;

namespace DT.Inventories.Domain.Contracts.Repositories;

public interface IInventoryRepository
{
    // main operations with products
    Task<Product?> GetProductByIdAsync(Guid productId);
    Task<Product?> GetProductBySkuAsync(string sku);
    
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(Guid productId);
    
    // operations with remains
    Task<InventoryItem?> GetInventoryItemAsync(Guid productId, Guid warehouseId);
    Task<IEnumerable<InventoryItem>> GetInventoryItemsByProductIdAsync(Guid productId);
    Task UpdateInventoryItemAsync(InventoryItem inventoryItem);
    
    // reservation
    Task ReserveItemsAsync(Guid orderId, IDictionary<Guid, int> items);
    Task ReleaseReservationAsync(Guid orderId);
    Task<IEnumerable<Reservation>> GetActiveReservationsAsync(Guid orderId);
    
    // movement
    Task AddInventoryMovementAsync(InventoryMovement inventoryMovement);
    Task<IEnumerable<InventoryMovement>> GetInventoryMovementsHistoryAsync(Guid productId, DateTime? fromDate, DateTime? toDate);
    
    // operations with warehouses
    Task<Warehouse?> GetWarehouseAsync(Guid warehouseId);
    Task AddWarehouseAsync(Warehouse warehouse);
}