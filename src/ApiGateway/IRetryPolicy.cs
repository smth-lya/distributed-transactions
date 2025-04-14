public interface IRetryPolicy
{
    Task ExecuteAsync(Func<Task> action);
}
