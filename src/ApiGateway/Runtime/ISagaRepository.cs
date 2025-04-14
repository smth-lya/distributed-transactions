public interface ISagaRepository<TState> where TState: ISagaState
{
    Task<TState?> LoadAsync(Guid correlactionId, CancellationToken cancellationToken = default);
    Task SaveAsync(TState state,  CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid correlactionId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid correlactionId, CancellationToken cancellationToken = default);
}
