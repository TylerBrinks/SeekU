using System;
using System.Collections.Generic;
using System.Linq;

namespace SeekU.Eventing
{
    /// <summary>
    /// In memory event store used for testing purposes
    /// </summary>
    public class InMemoryEventStore : IEventStore
    {
        private static readonly Dictionary<Guid, List<DomainEvent>> Events = new Dictionary<Guid, List<DomainEvent>>();

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, long startVersion)
        {
            return Events.ContainsKey(aggregateRootId)
                ? Events[aggregateRootId].Where(e => e.Sequence >= startVersion)
                : new List<DomainEvent>();
        }

        public void Insert(Guid aggregateRootId, IEnumerable<DomainEvent> domainEvents)
        {
            var events = domainEvents.ToList();

            if (!events.Any())
            {
                return;
            }

            //var aggregateId = events.First().AggregateRootId;

            var queue = new List<DomainEvent>();

            if (Events.ContainsKey(aggregateRootId))
            {
                queue = Events[aggregateRootId];
            }
            else
            {
                Events.Add(aggregateRootId, queue);                
            }

            queue.AddRange(events);
        }
    }
}
