using DT.Orders.Domain.Contracts.Repositories;
using DT.Orders.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DT.Orders.Infrastructure.Database.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;
    
    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }
    
    public async Task AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}