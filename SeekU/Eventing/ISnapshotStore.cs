using System;
using SeekU.Domain;

namespace SeekU.Eventing
{
    /// <summary>
    /// Represents a snapshot store for storing aggregate root snapshots
    /// </summary>
    public interface ISnapshotStore
    {
        Snapshot<T> GetSnapshot<T>(Guid aggregateRootId);
        void SaveSnapshot<T>(Snapshot<T> snapshot);
    }
}
