using System;

namespace SeekU.Azure
{
    public class SnapshotDetail
    {
        public long Id { get; set; }
        public Guid AggregateRootId { get; set; }
        public long Version { get; set; }
        public object SnapshotData { get; set; }
    }
}