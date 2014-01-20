using System;
using System.Collections.Generic;

namespace SeekU.Eventing
{
    /// <summary>
    /// Represents an event store for storing aggregate root event streams
    /// </summary>
    public interface IEventStore
    {
        IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, long startVersion);
        void Insert(Guid aggregateRootId, IEnumerable<DomainEvent> domainEvents);
        //IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes);
        //IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, Guid aggregateRootId);
        //IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, DateTime startDate, DateTime endDate);
    }
}