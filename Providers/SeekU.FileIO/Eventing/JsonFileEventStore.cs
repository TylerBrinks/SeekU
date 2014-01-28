using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SeekU.Eventing;

namespace SeekU.FileIO.Eventing
{
    /// <summary>
    /// Saves event streams to files serialized in json format
    /// </summary>
    public class JsonFileEventStore : FileEventStoreBase, IEventStore
    {
        /// <summary>
        /// Serializer settings to preserve namespaces
        /// </summary>
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        /// <summary>
        /// Deserializes events stored in json format to an event eventStream
        /// </summary>
        /// <param name="text">Json serialized events</param>
        /// <returns>Event eventStream</returns>
        public override EventStream GetEventStream(string text)
        {
            return JsonConvert.DeserializeObject<EventStream>(text, SerializerSettings);
        }

        /// <summary>
        /// Serializes an event eventStream to json
        /// </summary>
        /// <param name="eventStream">Stream to serialize</param>
        /// <returns>Json serialized event eventStream</returns>
        public override string GetEventStreamText(EventStream eventStream)
        {
            return JsonConvert.SerializeObject(eventStream, SerializerSettings);
        }

        /// <summary>
        /// Gets all events stored on disk
        /// </summary>
        /// <param name="aggregateRootId">Aggregate rood ID to query</param>
        /// <param name="startVersion">Starting event versio</param>
        /// <returns>List of domain events</returns>
        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, long startVersion)
        {
            return GetAllEvents(aggregateRootId, startVersion);
        }

        /// <summary>
        /// Saves a new file to disk containing event eventStream data
        /// </summary>
        /// <param name="aggregateRootId">Aggregate root id</param>
        /// <param name="domainEvents">List of events</param>
        public void Insert(Guid aggregateRootId, IEnumerable<DomainEvent> domainEvents)
        {
            InsertEvents(aggregateRootId, domainEvents, "json");
        }
    }
}
