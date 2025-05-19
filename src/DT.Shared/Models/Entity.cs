namespace DT.Shared.Models;

public class Entity<TId> where TId : IEquatable<TId>
{
    protected Entity(TId id)
    {
        Id = id;
    }

    private TId Id { get; }

    
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;
        
        if (ReferenceEquals(this, other))
            return true;
        
        if (GetType() != other.GetType())
            return false;
        
        return Id.Equals(other.Id);
    }

    private bool Equals(Entity<TId> other)
        => EqualityComparer<TId>.Default.Equals(Id, other.Id);

    public override int GetHashCode()
        => EqualityComparer<TId>.Default.GetHashCode(Id);

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (left is null && right is null)
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);
}