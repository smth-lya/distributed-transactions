using System.Collections.Concurrent;
using ApiGateway.Core;

public class SagaContext : ISagaContext, IDisposable
{
    private readonly ConcurrentDictionary<string, object> _data = new();
    private readonly List<IDisposable> _disposables = new();
    
    public Guid CorrelationId { get; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
    public CancellationToken CancellationToken { get; init; } = default;

    public string SagaName { get; }

    public string StepName { get; }

    public IMetadataStore Metadata { get; }

    public IEventAwaiter EventAwaiter { get; }

    public ITracingContext TracingContext { get; }

    public void SetData<T>(string key, T value) 
        => _data[key] = value;

    public T GetData<T>(string key) 
        => _data.TryGetValue(key, out var val) ? (T)val : default;

    public bool TryGetData<T>(string key, out T value)
    {
        var exists = _data.TryGetValue(key, out var val);
        value = exists ? (T)val : default;
        return exists;
    }

    public async Task<TEvent> WaitForEventAsync<TEvent>(
        TimeSpan timeout, 
        CancellationToken ct = default) where TEvent : class
    {
        var tcs = new TaskCompletionSource<TEvent>();
        var cts = CancellationTokenSource.CreateLinkedTokenSource(ct, CancellationToken);
        cts.CancelAfter(timeout);
        
        var registration = cts.Token.Register(() => 
            tcs.TrySetCanceled(cts.Token));
        
        _disposables.Add(registration);
        _disposables.Add(cts);

        return await tcs.Task;
    }

    public void Dispose()
    {
        foreach (var disposable in _disposables)
            disposable.Dispose();
    }

    public bool IsStepCompleted(string stepName)
    {
        throw new NotImplementedException();
    }

    public void MarkStepCompleted(string stepName)
    {
        throw new NotImplementedException();
    }
}