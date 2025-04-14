public class ExponentialBackoffRetry : IRetryPolicy
{
    private readonly int _maxRetries;
    private readonly ILogger<ExponentialBackoffRetry> _logger;

    public ExponentialBackoffRetry(
        int maRetries, 
        ILogger<ExponentialBackoffRetry> logger)
    {
        _maxRetries = maRetries;
        _logger = logger;
    }

    public async Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        var attempt = 0;
        while (true)
        {
            try
            {
                await action();
                return;
            }
            catch (Exception ex) when (IsTransient(ex) && attempt < _maxRetries)
            {
                attempt++;

                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));

                _logger?.LogWarning(ex,
                    "Retry attempt {Attempt} after {Delay}ms",
                    attempt, delay.TotalMilliseconds);

                await Task.Delay(delay + RandomJitter(), cancellationToken);
            }
        }
    }
    
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        var attempt = 0;
        while (true)
        {
            try
            {
                return await action();
            }
            catch (Exception ex) when (IsTransient(ex) && attempt < _maxRetries)
            {
                attempt++;

                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));

                _logger?.LogWarning(ex,
                    "Retry attempt {Attempt} after {Delay}ms",
                    attempt, delay.TotalMilliseconds);

                await Task.Delay(delay + RandomJitter(), cancellationToken);
            }
        }
    }
    
    private static bool IsTransient(Exception ex)
        => ex is TimeoutException or HttpRequestException;
    private static TimeSpan RandomJitter()
        => TimeSpan.FromMilliseconds(Random.Shared.Next(100, 500));
}
