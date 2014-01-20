using System;
using System.Collections.Generic;
using System.Diagnostics;
using ReflectionMagic;
using SeekU.Domain;
using SeekU.Eventing;

namespace SeekU.Commanding
{
    public class CommandContext
    {
        private readonly DomainRepository _repository;
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;
        private readonly ISnapshotStore _snapshotStore;
 
        /// <summary>
        /// Initializes a command context unit of work
        /// </summary>
        /// <param name="dependencyResolver">Object for resolving dependencies</param>
        [DebuggerStepThrough]
        public CommandContext(IDependencyResolver dependencyResolver)
        {
            _eventStore = dependencyResolver.Resolve<IEventStore>();
            _eventBus = dependencyResolver.Resolve<IEventBus>();
            _snapshotStore = dependencyResolver.Resolve<ISnapshotStore>();
            _repository = new DomainRepository(_snapshotStore, _eventStore); //dependencyResolver.Resolve<IDomainRepository>();
        }

        /// <summary>
        /// Get's a persisted aggregate root by a given id
        /// </summary>
        /// <typeparam name="T">Type of aggregate root to load</typeparam>
        /// <param name="id">Id of the root object</param>
        /// <returns>Aggregate root of type T with events and snapshots replayed</returns>
        public T GetById<T>(Guid id) where T : AggregateRoot, new()
        {
            return _repository.GetById<T>(id);
        }

        /// <summary>
        /// Persists event stream, publishes events, and creates snapshots
        /// </summary>
        /// <param name="root">Aggregate root to persist</param>
        public void Finalize(AggregateRoot root)
        {
            // Persist events to the event store
            _eventStore.Insert(root.Id, root.AppliedEvents);

            // Publish events to interested parties
            _eventBus.PublishEvents(root.AppliedEvents);

            CreateSnapshot(root);
        }

        /// <summary>
        /// Optionally reates a snapshot of the aggregate root
        /// </summary>
        /// <param name="aggregateRoot">Aggregate root instance to snapshot</param>
        internal void CreateSnapshot(AggregateRoot aggregateRoot)
        {
            if ( !(aggregateRoot is IAggregateRootWithSnapshot))
            {
                return;
            }

            var dynamicRoot = aggregateRoot.AsDynamic();

            // Only create a snapshot of the root requires it
            if (!dynamicRoot.ShouldCreateSnapshot())
            {
                return;
            }

            var snapshot = ((IAggregateRootWithSnapshot) aggregateRoot).CreateGenericSnapshot();

            if (snapshot == null)
            {
                return;
            }

            // Dynamically invoke the SaveSnapshot<T> method of the snapshot store
            _snapshotStore.InvokeGenericMethod("SaveSnapshot", ((IAggregateRootWithSnapshot) aggregateRoot).GetGenericType(), snapshot);
        }
    }
}