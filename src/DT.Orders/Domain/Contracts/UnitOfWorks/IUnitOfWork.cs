using DT.Orders.Domain.Contracts.Repositories;

namespace DT.Orders.Domain.Contracts.UnitOfWorks;

public interface IUnitOfWork : IDisposable
{
    IOrderRepository Orders { get; }
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}