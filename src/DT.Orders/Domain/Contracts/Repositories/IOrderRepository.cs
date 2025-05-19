using DT.Orders.Domain.Models;

namespace DT.Orders.Domain.Contracts.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId);
    
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
}