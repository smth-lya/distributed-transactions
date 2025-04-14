public interface ISagaRepository<TState> where TState: ISagaState
{
    Task<TState> LoadAsync(Guid correlactionId);
    Task SaveAsync(TState state);
    Task DeleteAsync(Guid correlactionId);
}
