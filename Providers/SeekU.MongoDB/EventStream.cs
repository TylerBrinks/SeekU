using System;
using MongoDB.Bson;

namespace SeekU.MongoDB
{
    public class EventStream
    {
        public ObjectId Id { get; set; }
        public long SequenceStart { get; set; }
        public long SequenceEnd { get; set; }
        public Guid AggregateRootId { get; set; }
        public DateTime DateCreated { get; set; }
        public dynamic EventData { get; set; }
    }
}
