using System;

namespace SeekU.FileIO
{
    public class SnapshotDetail
    {
        public long Id { get; set; }
        public Guid AggregateRootId { get; set; }
        public long Version { get; set; }
        public object Data { get; set; }
    }
}