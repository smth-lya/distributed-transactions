using DT.Orders.Domain.Contracts.Repositories;
using DT.Orders.Domain.Enums;
using DT.Orders.Domain.Exceptions;
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
            .Include(o => o.StatusChanges)
            .AsSplitQuery()
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.Orders
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
    {
        return await _context.Orders
            .Where(o => o.Status == status)
            .Include(o => o.Items)
            .ToListAsync();
    }

    public async Task AddAsync(Order order)
    {
        var loadedOrder = await _context.Orders.FindAsync(order.Id);
        if (loadedOrder is not null)
        {
            throw new Exception("Order already exists");
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order is null)
        {
            throw new OrderNotFoundException(orderId);
        }
     
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }
}