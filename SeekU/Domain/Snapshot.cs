using System;

namespace SeekU.Domain
{
    /// <summary>
    /// Snapshots capture the state of an aggregate root at a moment in time.  When an 
    /// aggregate root is reloaded, it can be replayed from the snapshot forward
    /// insted of replaying the entire event stream.
    /// </summary>
    /// <typeparam name="T">Type of object containing snapshot data</typeparam>
    public class Snapshot<T>
    {
        public Snapshot()
        {
        }

        public Snapshot(Guid aggregateRoodId, long version, T data)
        {
            AggregateRootId = aggregateRoodId;
            Version = version;
            Data = data;
        }

        public Guid AggregateRootId { get; set; }
        public long Version { get; set; }
        public T Data { get; set; }
    }
}