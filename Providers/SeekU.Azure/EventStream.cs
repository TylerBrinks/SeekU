using System;
using System.Globalization;
using Microsoft.WindowsAzure.Storage.Table;

namespace SeekU.Azure
{
    public class EventStream : TableEntity
    {
        public EventStream()
        {
            
        }

        public EventStream(Guid aggregateRootId, long sequenceStart)
        {
            PartitionKey = aggregateRootId.ToString();
            RowKey = sequenceStart.ToString(CultureInfo.InvariantCulture);
        }

        public long SequenceEnd { get; set; }
        public string EventData { get; set; }
    }
}