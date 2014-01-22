using System;
using System.Collections.Generic;

namespace SeekU.Azure
{
    /// <summary>
    /// Represents event stream and snapshot persistance for Azure blobs
    /// </summary>
    public interface IAzureStorageRepository
    {
        List<EventStream> GetEventStream(Guid aggregateRoodId, long startVersion);
        void InsertEvents(EventStream events);
        SnapshotDetail GetSnapshot(Guid aggregateRootId);
        void InsertSnapshot(SnapshotDetail snapshot);
    }
}