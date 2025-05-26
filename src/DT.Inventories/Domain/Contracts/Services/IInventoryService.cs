namespace DT.Inventories.Domain.Contracts.Services;

public interface IInventoryService
{
    Task<bool> ReserveItemAsync(Guid orderId, IDictionary<Guid, int> items);
    
    Task<bool> ReleaseItemsAsync(Guid orderId);
}