using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SeekU.Eventing;

namespace SeekU.Sql.Eventing
{
    /// <summary>
    /// SQL event storage provider
    /// </summary>
    public class SqlEventStore : IEventStore
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        public Func<ISqlRepository> GetRepository = () => new SqlRepository();

        /// <summary>
        /// Globally sets the name of the event streams connection string for SQL 
        /// </summary>
        public string ConnectionStringName
        {
            get { return SqlRepository.EventConnectionStringName; }
            set { SqlRepository.EventConnectionStringName = value; }
        }

        /// <summary>
        /// Queries SQL for a stream of events for a given ID.
        /// </summary>
        /// <param name="aggregateRootId">ID of the aggregate root instance</param>
        /// <param name="startVersion">Starting event version of the event sequence range.</param>
        /// <returns>Ordered list of domain events</returns>
        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, long startVersion)
        {
            var events = new List<DomainEvent>();

            var streams = GetRepository().GetEventStream(aggregateRootId, startVersion);

            foreach (var stream in streams.OrderBy(evt => evt.SequenceStart))
            {
                var streamEvents = JsonConvert.DeserializeObject<IEnumerable<DomainEvent>>(stream.EventData, SerializerSettings);
                events.AddRange(streamEvents);
            }

            return events;
        }

        /// <summary>
        /// Inserts a new list of events into SQL
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
                EventData = JsonConvert.SerializeObject((dynamic)events,  SerializerSettings)
            };

            GetRepository().InsertEvents(stream);
        }
    }
}
