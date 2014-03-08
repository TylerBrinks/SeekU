using System;
using System.Runtime.Serialization;

namespace SeekU.Eventing
{
    /// <summary>
    /// Represents an event fired by an aggregate root domain object
    /// </summary>
    [DataContract]
    public abstract class DomainEvent
    {
        [DataMember]
        public long Sequence { get; set; }

        [DataMember]
        public DateTime EventDate { get; set; }

        public virtual DomainEvent UpgradeVersion()
        {
            return null;
        }
    }
}
