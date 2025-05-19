using DT.Inventories.Domain.Contracts.Repositories;
using DT.Inventories.Domain.Contracts.Services;

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
        foreach (var item in items)
        {
            var stock = await _inventoryRepository.GetByProductIdAsync(item.Key);
            if (stock == null || stock.Quantity < item.Value)
                return false;
            
            stock.Reserve(item.Value);
            await _inventoryRepository.UpdateAsync(stock);
        }
        
        return true;
    }

    public async Task<bool> ReleaseItemsAsync(Guid orderId, IDictionary<Guid, int> items)
    {
        foreach (var item in items)
        {
            var stock = await _inventoryRepository.GetByProductIdAsync(item.Key);
            if (stock == null || stock.Reserved < item.Value)
                return false;
            
            stock.Release(item.Value);
            await _inventoryRepository.UpdateAsync(stock);
        }
        
        return true;
    }
}