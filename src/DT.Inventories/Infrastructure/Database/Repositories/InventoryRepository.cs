using DT.Inventories.Domain.Contracts.Repositories;
using DT.Inventories.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DT.Inventories.Infrastructure.Database.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private InventoryDbContext _context;
    
    public InventoryRepository(InventoryDbContext context)
    {
        _context = context;
    }
    
    public async Task<Stock?> GetByProductIdAsync(Guid productId)
    {
        return await _context.Stocks
            .FirstOrDefaultAsync(s => s.ProductId == productId);
    }

    public async Task AddAsync(Stock stock)
    {
        _context.Stocks.Add(stock);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Stock stock)
    {
        _context.Stocks.Update(stock);  
        await _context.SaveChangesAsync();
    }
}