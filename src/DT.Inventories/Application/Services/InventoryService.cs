using DT.Inventories.Domain.Contracts.Repositories;
using DT.Inventories.Domain.Contracts.Services;
using DT.Inventories.Domain.Models;

namespace DT.Inventories.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(IInventoryRepository inventoryRepository, ILogger<InventoryService> logger)
    {
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }
    
    public async Task<bool> ReserveItemAsync(Guid orderId, IDictionary<Guid, int>  items)
    {
        try
        {
            var copyOfItems = items.ToDictionary(k => k.Key, v => v.Value);
            await _inventoryRepository.ReserveItemsAsync(orderId, copyOfItems);
            
            _logger.LogInformation("Successfully reserved items for OrderId: {OrderId}", orderId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reserve items for OrderId: {OrderId}", orderId);
            return false;
        }
    }

    public async Task<bool> ReleaseItemsAsync(Guid orderId)
    {
        _logger.LogInformation("Attempting to release reserved items for OrderId: {OrderId}", orderId);

        try
        {
            await _inventoryRepository.ReleaseReservationAsync(orderId);
            _logger.LogInformation("Successfully released reservation for OrderId: {OrderId}", orderId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to release reservation for OrderId: {OrderId}", orderId);
            return false;
        }
    }
}