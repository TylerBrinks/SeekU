using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SeekU.Eventing;

namespace SeekU.Sql.Eventing
{
    public class SqlEventStore : IEventStore
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        public Func<ISqlDatabase> GetDatabase = () => new SqlDatabase(); 

        public string ConnectionStringName
        {
            get { return SqlDatabase.EventConnectionStringName; }
            set { SqlDatabase.EventConnectionStringName = value; }
        }

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, long startVersion)
        {
            var events = new List<DomainEvent>();

            var streams = GetDatabase().GetEventStream(aggregateRootId, startVersion);

            foreach (var stream in streams.OrderBy(evt => evt.SequenceStart))
            {
                var streamEvents = JsonConvert.DeserializeObject<IEnumerable<DomainEvent>>(stream.EventData, SerializerSettings);
                events.AddRange(streamEvents);
            }

            return events;
        }

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

            GetDatabase().InsertEvents(stream);
        }
    }
}
