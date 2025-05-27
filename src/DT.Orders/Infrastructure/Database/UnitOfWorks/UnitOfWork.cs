using DT.Orders.Domain.Contracts.Repositories;
using DT.Orders.Domain.Contracts.UnitOfWorks;

namespace DT.Orders.Infrastructure.Database.UnitOfWorks;

public class UnitOfWork : IUnitOfWork
{
    private readonly OrderDbContext _context;
    private readonly IOrderRepository _orderRepository;

    public UnitOfWork(OrderDbContext context, IOrderRepository orderRepository)
    {
        _context = context;
        _orderRepository = orderRepository;
    }
    
    public IOrderRepository Orders => _orderRepository;
    
    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}