using System;
using System.Collections.Generic;
using System.Linq;
using SeekU.Eventing;

namespace SeekU.MongoDB.Eventing
{
    /// <summary>
    /// MongoDB event storage provider
    /// </summary>
    public class MongoEventStore : IEventStore
    {
        public Func<IMongoRepository> GetRepository = () => new MongoRepository();

        /// <summary>
        /// Globally sets the name of the event streams connection string for MongoDB 
        /// </summary>
        public string ConnectionStringName
        {
            get { return MongoRepository.EventConnectionStringName; }
            set { MongoRepository.EventConnectionStringName = value; }
        }

        /// <summary>
        /// Globally sets the name of the event stream database in MongoDB
        /// </summary>
        public string DatabaseName
        {
            get { return MongoRepository.EventDatabaseName; }
            set { MongoRepository.EventDatabaseName = value; }
        }

        /// <summary>
        /// Queries MongoDB for a stream of events for a given ID.
        /// </summary>
        /// <param name="aggregateRootId">ID of the aggregate root instance</param>
        /// <param name="startVersion">Starting event version of the event sequence range.</param>
        /// <returns>Ordered list of domain events</returns>
        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, long startVersion)
        {
            var events = new List<DomainEvent>();
            var streams = GetRepository().GetEventStream(aggregateRootId, startVersion);

            var eventStream = streams.OrderBy(evt => evt.SequenceStart).Select(stream => stream.EventData);

            foreach (var streamEvents in eventStream)
            {
                events.AddRange(streamEvents);
            }

            return events;
        }

        /// <summary>
        /// Inserts a new list of events into MongoDB
        /// </summary>
        /// <param name="aggregateRootId">Aggregate root ID</param>
        /// <param name="domainEvents">List of events to insert</param>
        public void Insert(Guid aggregateRootId, IEnumerable<DomainEvent> domainEvents)
        {
            var events = domainEvents.ToList();

            var firstEvent = events.First();
            var lastEvent = events.Last();

            var stream = new EventStream
            {
                AggregateRootId = aggregateRootId,
                DateCreated = DateTime.UtcNow,
                SequenceStart = firstEvent.Sequence,
                SequenceEnd = lastEvent.Sequence,
                EventData = domainEvents
            };

            GetRepository().InsertEvents(stream);
        }
    }
}
