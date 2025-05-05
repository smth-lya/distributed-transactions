namespace DT.Saga.Core;

public interface IMetadataStore
{
    void Set<T>(string key, T value);
    T Get<T>(string key);
    bool TryGet<T>(string key, out T value);
}