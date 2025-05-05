namespace DT.Saga.Core;

public interface IEventAwaiter
{
    Task<TEvent> WaitForEventAsync<TEvent>(TimeSpan timeout, CancellationToken ct = default) 
        where TEvent : class;
}