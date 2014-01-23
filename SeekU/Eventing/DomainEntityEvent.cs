using System;
using System.Runtime.Serialization;

namespace SeekU.Eventing
{
    /// <summary>
    /// Represents an event fired by entites contained within an aggregate
    /// </summary>
    public abstract class DomainEntityEvent : DomainEvent
    {
        [DataMember]
        public Guid EntityId { get; set; }
    }
}