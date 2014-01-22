using System;
using System.Collections.Generic;

namespace SeekU.MongoDB
{
    /// <summary>
    /// Represents event stream and snapshot persistance for MongoDB
    /// </summary>
    public interface IMongoDataStore
    {
        List<EventStream> GetEventStream(Guid aggregateRoodId, long startVersion);
        void InsertEvents(EventStream events);
        SnapshotDetail GetSnapshot(Guid aggregateRootId);
        void InsertSnapshot(SnapshotDetail snapshot);
    }
}