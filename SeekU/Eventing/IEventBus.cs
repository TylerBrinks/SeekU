using System.Collections.Generic;

namespace SeekU.Eventing
{
    /// <summary>
    /// Represents an event bus responsible for publishing events to event handlers
    /// </summary>
    public interface IEventBus
    {
        void PublishEvent(DomainEvent domainEvent);
        void PublishEvents(IEnumerable<DomainEvent> domainEvents);
    }
}
