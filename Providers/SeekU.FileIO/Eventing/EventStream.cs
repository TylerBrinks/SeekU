using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SeekU.Eventing;

namespace SeekU.FileIO.Eventing
{
    [DataContract]
    public class EventStream
    {
        [DataMember]
        public long SequenceStart { get; set; }
        [DataMember]
        public long SequenceEnd { get; set; }
        [DataMember]
        public Guid AggregateRootId { get; set; }
        [DataMember]
        public DateTime DateCreated { get; set; }
        [DataMember]
        public IEnumerable<DomainEvent> EventData { get; set; }
    }
}
