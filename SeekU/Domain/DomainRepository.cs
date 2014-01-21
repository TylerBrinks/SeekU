using System;
using ReflectionMagic;
using SeekU.Eventing;

namespace SeekU.Domain
{
    public class DomainRepository
    {
        private readonly ISnapshotStore _snapshotStore;
        private readonly IEventStore _eventStore;

        /// <summary>
        /// Repository for loading aggregate root objects from a stream
        /// of events and snapshots
        /// </summary>
        /// <param name="snapshotStore">Snapshot storage object</param>
        /// <param name="eventStore">Event storage object</param>
        public DomainRepository(ISnapshotStore snapshotStore, IEventStore eventStore)
        {
            _snapshotStore = snapshotStore;
            _eventStore = eventStore;
        }

        /// <summary>
        /// Gets an aggregate root instance of type T for a given ID
        /// </summary>
        /// <typeparam name="T">Type of class to create and hydrate</typeparam>
        /// <param name="aggregateRootId">ID of the persisted aggregate root instance</param>
        /// <returns></returns>
        public T GetById<T>(Guid aggregateRootId) where T : AggregateRoot, new()
        {
            var aggregateRoot = new T();

            // Attempts to load the most recent snapshot for the given ID
            var snapshot = GetSnapshot(aggregateRoot, aggregateRootId);

            if (snapshot != null)
            {
                // Call the LoadFromSnapshot method dynamically with the generic snapshot instance
                GetType().AsDynamicType().LoadFromSnapshot(aggregateRoot, snapshot);
            }

            // Gets the event stream for the given ID.  Events are loaded based on the
            // root's current version which allows for replaying only a subset of events
            // after a snapshot was taken.
            var events = _eventStore.GetEvents(aggregateRootId, aggregateRoot.Version + 1);

            // Replays all events to bring the root up to current version
            aggregateRoot.ReplayEvents(events);

            return aggregateRoot.Version == 0 
                ? null
                : aggregateRoot;
        }

        /// <summary>
        /// Loads a snapshot from the snapshot store and initializes the aggregate
        /// root's history from the snapshot's data bringing the aggregate root up
        /// to the current version of the snapshot.
        /// </summary>
        /// <param name="aggregateRoot">Snapshot-capable aggregate root.</param>
        /// <param name="snapshot">Snapshot of generic type T to dynamically load</param>
        private static void LoadFromSnapshot(AggregateRoot aggregateRoot, object snapshot)
        {
            if (!(aggregateRoot is IAggregateRootWithSnapshot))
            {
                return;
            }

            var dynamicRoot = aggregateRoot.AsDynamic();
            
            // Initiazlie the root which will set the current version
            dynamicRoot.InitializeFromSnapshot(snapshot);

            // Bring the root's state up to the snapshot's state and version
            dynamicRoot.LoadFromSnapshot(snapshot.AsDynamic().Data);
        }

        /// <summary>
        /// Calls the generic GetSnapshot method of the snapshot store object.
        /// </summary>
        /// <param name="root">Snapshot-capable aggregate Root to load</param>
        /// <param name="aggregateRoodId">ID of the persisted aggregate root instance</param>
        /// <returns>Snapshot of Type T casted as an object</returns>
        private object GetSnapshot(AggregateRoot root, Guid aggregateRoodId)
        {
            if (!(root is IAggregateRootWithSnapshot))
            {
                return null;
            }

            var casted = (IAggregateRootWithSnapshot)root;

            var snapshotType = casted.GetGenericType();
            var method = _snapshotStore.GetType().GetMethod("GetSnapshot").MakeGenericMethod(snapshotType);

            return method.Invoke(_snapshotStore, new object[] { aggregateRoodId });
        }
    }
}