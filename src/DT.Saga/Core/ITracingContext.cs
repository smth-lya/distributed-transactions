namespace DT.Saga.Core;

public interface ITracingContext
{
    string TraceId { get; }
    void SetBaggage(string key, string value);
    string? GetBaggage(string key);
}