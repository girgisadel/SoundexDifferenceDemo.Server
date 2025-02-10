using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoundexDifferenceDemo.SharedKernel;

public abstract class Entity : IEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    [JsonIgnore]
    [NotMapped]
    public virtual List<IDomainEvent> DomainEvents => [.. _domainEvents];

    public virtual void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public virtual void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
