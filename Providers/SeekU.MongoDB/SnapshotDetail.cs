using System;
using MongoDB.Bson;

namespace SeekU.MongoDB
{
    public class SnapshotDetail
    {
        public ObjectId Id { get; set; }
        public Guid AggregateRootId { get; set; }
        public long Version { get; set; }
        public object SnapshotData { get; set; }
    }
}