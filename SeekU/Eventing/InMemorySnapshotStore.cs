using System;
using System.Collections.Generic;
using SeekU.Domain;

namespace SeekU.Eventing
{
    /// <summary>
    /// In memory snapshot store used for testing
    /// </summary>
    public class InMemorySnapshotStore : ISnapshotStore
    {
        private readonly static Dictionary<Guid, object> _snapshots = new Dictionary<Guid, object>();
 
        public Snapshot<T> GetSnapshot<T>(Guid aggregateRootId)
        {
            return _snapshots.ContainsKey(aggregateRootId)
                ? (Snapshot<T>)_snapshots[aggregateRootId]
                : null;
        }

        public void SaveSnapshot<T>(Snapshot<T> snapshot) 
        {
            _snapshots[snapshot.AggregateRootId] = snapshot;
        }
    }
}