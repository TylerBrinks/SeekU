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
        private readonly static Dictionary<Guid, object> Snapshots = new Dictionary<Guid, object>();
 
        public Snapshot<T> GetSnapshot<T>(Guid aggregateRootId)
        {
            return Snapshots.ContainsKey(aggregateRootId)
                ? (Snapshot<T>)Snapshots[aggregateRootId]
                : null;
        }

        public void SaveSnapshot<T>(Snapshot<T> snapshot) 
        {
            Snapshots[snapshot.AggregateRootId] = snapshot;
        }
    }
}