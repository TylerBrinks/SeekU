//using System;
//using System.Collections.Generic;
//using System.Linq;
//using SeekU.Eventing;

//namespace SeekU.Azure.Eventing
//{
//    /// <summary>
//    /// Azure table event storage provider
//    /// </summary>
//    public class AzureBlobEventStore : IEventStore
//    {
//        public Func<IAzureBlobDataStore> GetRepository = () => new AzureBlobDataStore();

//        /// <summary>
//        /// Globally sets the name of the event streams connection string for Azure 
//        /// </summary>
//        public string ConnectionStringName
//        {
//            get { return AzureBlobDataStore.EventConnectionStringName; }
//            set { AzureBlobDataStore.EventConnectionStringName = value; }
//        }

//        /// <summary>
//        /// Globally sets the name of the event stream database in Azure
//        /// </summary>
//        public string DatabaseName
//        {
//            get { return AzureBlobDataStore.EventContainerName; }
//            set { AzureBlobDataStore.EventContainerName = value; }
//        }

//        /// <summary>
//        /// Queries Azure for a stream of events for a given ID.
//        /// </summary>
//        /// <param name="aggregateRootId">ID of the aggregate root instance</param>
//        /// <param name="startVersion">Starting event version of the event sequence range.</param>
//        /// <returns>Ordered list of domain events</returns>
//        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, long startVersion)
//        {
//            var events = new List<DomainEvent>();
//            var streams = GetRepository().GetEventStream(aggregateRootId, startVersion);

//            var eventStream = streams.OrderBy(evt => evt.SequenceStart).Select(stream => stream.EventData);

//            foreach (var streamEvents in eventStream)
//            {
//                events.AddRange(streamEvents);
//            }

//            return events;
//        }

//        /// <summary>
//        /// Inserts a new list of events into Azure
//        /// </summary>
//        /// <param name="aggregateRootId">Aggregate root ID</param>
//        /// <param name="domainEvents">List of events to insert</param>
//        public void Insert(Guid aggregateRootId, IEnumerable<DomainEvent> domainEvents)
//        {
//            var events = domainEvents.ToList();

//            var firstEvent = events.First();
//            var lastEvent = events.Last();

//            var stream = new EventStream
//            {
//                AggregateRootId = aggregateRootId,
//                DateCreated = DateTime.UtcNow,
//                SequenceStart = firstEvent.Sequence,
//                SequenceEnd = lastEvent.Sequence,
//                EventData = domainEvents
//            };

//            GetRepository().InsertEvents(stream);
//        }
//    }
//}
