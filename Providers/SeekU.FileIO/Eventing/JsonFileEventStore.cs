using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SeekU.Eventing;

namespace SeekU.FileIO.Eventing
{
    public class JsonFileEventStore : FileEventStore, IEventStore
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        public override EventStream GetEventStream(string text)
        {
            return JsonConvert.DeserializeObject<EventStream>(text, SerializerSettings);
        }

        public override string GetEventStreamText(EventStream stream)
        {
            return JsonConvert.SerializeObject(stream, SerializerSettings);
        }

        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, long startVersion)
        {
            return GetAllEvents(aggregateRootId, startVersion);
        }

        public void Insert(Guid aggregateRootId, IEnumerable<DomainEvent> domainEvents)
        {
            InsertEvents(aggregateRootId, domainEvents, "json");
        }
    }
}
