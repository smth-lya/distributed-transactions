using System.Data;
using DT.Inventories.Domain.Contracts.Repositories;
using DT.Inventories.Domain.Enums;
using DT.Inventories.Domain.Exceptions;
using DT.Inventories.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DT.Inventories.Infrastructure.Database.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<InventoryRepository> _logger;
    
    public InventoryRepository(
        InventoryDbContext context, 
        ILogger<InventoryRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    #region Product Operations
    public async Task<Product?> GetProductByIdAsync(Guid productId)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<Product?> GetProductBySkuAsync(string sku)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.SKU == sku);
    }

    public async Task AddProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Guid productId)
    {
        await _context.Products
            .Where(p => p.Id == productId)
            .ExecuteDeleteAsync();
    }
    
    #endregion
    
    #region Inventory Operations
    public async Task<InventoryItem?> GetInventoryItemAsync(Guid productId, Guid warehouseId)
    {
        return await _context.InventoryItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == productId && i.WarehouseId == warehouseId);
    }

    public async Task<IEnumerable<InventoryItem>> GetInventoryItemsByProductIdAsync(Guid productId)
    {
        return await _context.InventoryItems
            .AsNoTracking()
            .Where(i => i.ProductId == productId)
            .ToListAsync();
    }

    public async Task UpdateInventoryItemAsync(InventoryItem inventoryItem)
    {
        _context.InventoryItems.Update(inventoryItem);
        await _context.SaveChangesAsync();
    }
    #endregion

    #region Reservation Operations
    public async Task ReserveItemsAsync(Guid orderId, IDictionary<Guid, int> items)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            foreach (var (productId, quantity) in items)
            {
                // Check and update inventory
                var inventoryItem = await _context.InventoryItems
                    .Where(i => i.ProductId == productId)
                    .FirstAsync();

                
                
                if (inventoryItem.Quantity - inventoryItem.Reserved < quantity)
                {
                    throw new InsufficientStockException(productId, 
                        inventoryItem.Quantity - inventoryItem.Reserved, 
                        quantity);
                }
                
                inventoryItem.Reserved += quantity;
                _context.InventoryItems.Update(inventoryItem);
                
                // create reservation
                var reservation = new Reservation
                {
                    OrderId = orderId,
                    ProductId = productId,
                    Quantity = quantity,
                    Status = ReservationStatus.Active,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                };

                await _context.Reservations.AddAsync(reservation);

                // record movement
                await _context.InventoryMovements.AddAsync(new InventoryMovement()
                {
                    ProductId = productId,
                    WarehouseId = inventoryItem.WarehouseId,
                    QuantityDelta = -quantity,
                    MovementType = MovementType.Reservation,
                    RelatedOrderId = orderId
                });
            }
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to reserve items for order {OrderId}", orderId);
            throw;
        }
    }

    public async Task ReleaseReservationAsync(Guid orderId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var reservations = await _context.Reservations
                .Where(r => r.OrderId == orderId && r.Status == ReservationStatus.Active)
                .ToListAsync();

            foreach (var reservation in reservations)
            {
                // update inventory
                var inventoryItem = _context.InventoryItems
                    .First(i => i.ProductId == reservation.ProductId);
                
                inventoryItem.Reserved -= reservation.Quantity;
                _context.InventoryItems.Update(inventoryItem);

                // update reservation status
                reservation.Status = ReservationStatus.Cancelled;
                _context.Reservations.Update(reservation);
                
                // record movement
                await _context.InventoryMovements.AddAsync(new InventoryMovement()
                {
                    ProductId = reservation.ProductId,
                    WarehouseId = inventoryItem.WarehouseId,
                    QuantityDelta = reservation.Quantity,
                    MovementType = MovementType.ReservationCancellation,
                    RelatedOrderId = orderId,
                });
            }
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to release reservation for order {OrderId}", orderId);
            throw;
        }
    }

    public async Task<IEnumerable<Reservation>> GetActiveReservationsAsync(Guid orderId)
    {
        return await _context.Reservations
            .AsNoTracking()
            .Where(r => 
                r.OrderId == orderId && 
                r.Status == ReservationStatus.Active)
            .ToListAsync();
    }
    #endregion

    #region Movement Operations
    public async Task AddInventoryMovementAsync(InventoryMovement inventoryMovement)
    {
        _context.InventoryMovements.Add(inventoryMovement);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<InventoryMovement>> GetInventoryMovementsHistoryAsync(
        Guid productId, 
        DateTime? fromDate, 
        DateTime? toDate)
    {
        var query = _context.InventoryMovements
            .AsNoTracking()
            .Where(m => m.ProductId == productId);
        
        if (fromDate.HasValue)
            query = query.Where(m => m.CreatedAt >= fromDate);
        
        if (toDate.HasValue)
            query = query.Where(m => m.CreatedAt <= toDate);
        
        return await query
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }
    #endregion

    #region Warehouse Operations
    public async Task<Warehouse?> GetWarehouseAsync(Guid warehouseId)
    {
        return await _context.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == warehouseId);
    }

    public async Task AddWarehouseAsync(Warehouse warehouse)
    {
        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();
    }
    #endregion
}