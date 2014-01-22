using System;
using System.Collections.Generic;

namespace SeekU.Sql
{
    /// <summary>
    /// Represents event stream and snapshot persistance for SQL
    /// </summary>
    public interface ISqlDataStore
    {
        List<EventStream> GetEventStream(Guid aggregateRoodId, long startVersion);
        void InsertEvents(EventStream events);
        SnapshotDetail GetSnapshot(Guid aggregateRootId);
        void InsertSnapshot(SnapshotDetail snapshot);
    }
}