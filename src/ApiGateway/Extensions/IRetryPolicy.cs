public interface IRetryPolicy
{
    Task ExecuteAsync(Func<Task> action,  CancellationToken cancellationToken = default);
    Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default);
}
