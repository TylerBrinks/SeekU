using System;
using SeekU.Eventing;

namespace SeekU.Domain
{
    /// <summary>
    /// Represents an object contained within the aggregate root with its own identity
    /// and application of events
    /// </summary>
    public class Entity
    {
        public Entity(AggregateRoot parent, Guid entityId)
        {
            Id = entityId;
            Parent = parent;
            Parent.Associate(this);
        }

        protected void ApplyEvent(DomainEvent domainEvent)
        {
            Parent.ApplyEvent(domainEvent);
        }
        
        public Guid Id { get; protected set; }

        public AggregateRoot Parent { get; set; }
    }
}
