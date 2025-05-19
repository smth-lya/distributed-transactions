using DT.Inventories.Domain.Models;

namespace DT.Inventories.Domain.Contracts.Repositories;

public interface IInventoryRepository
{
    Task<Stock?> GetByProductIdAsync(Guid productId);

    Task AddAsync(Stock stock);
    Task UpdateAsync(Stock stock);
}