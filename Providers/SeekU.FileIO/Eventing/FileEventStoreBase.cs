using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SeekU.Eventing;

namespace SeekU.FileIO.Eventing
{
    /// <summary>
    /// Base class for file-base event storage
    /// </summary>
    public abstract class FileEventStoreBase
    {
        public abstract EventStream GetEventStream(string text);
        public abstract string GetEventStreamText(EventStream eventStream);

        /// <summary>
        /// Gets all events stored on disk
        /// </summary>
        /// <param name="aggregateRootId">Aggregate rood ID to query</param>
        /// <param name="startVersion">Starting event versio</param>
        /// <returns>List of domain events</returns>
        public IEnumerable<DomainEvent> GetAllEvents(Guid aggregateRootId, long startVersion)
        {
            var aggregateRootDirectory = Path.Combine(FileUtility.GetEventDirectory(), aggregateRootId.ToString());
            var paths = from filePath in Directory.GetFiles(aggregateRootDirectory)
                let fileName = Path.GetFileNameWithoutExtension(filePath)
                where fileName != null
                let sequence = int.Parse(fileName.Split('-')[0])
                where sequence >= startVersion
                select new { Sequence = sequence, FilePath = filePath };

            var domainEvents = new List<DomainEvent>();

            foreach (var eventInfo in paths)
            {
                var text = File.ReadAllText(eventInfo.FilePath);
                var domainEvent = GetEventStream(text);
                domainEvents.AddRange(domainEvent.EventData);
            }

            return domainEvents;
        }

        /// <summary>
        /// Saves a new file to disk containing event eventStream data
        /// </summary>
        /// <param name="aggregateRootId">Aggregate root id</param>
        /// <param name="domainEvents">List of events</param>
        /// <param name="extension">File extension for the created file</param>
        public void InsertEvents(Guid aggregateRootId, IEnumerable<DomainEvent> domainEvents, string extension)
        {
            var events = domainEvents.ToList();

            var firstEvent = events.First();
            var lastEvent = events.Last();

            var stream = new EventStream
            {
                AggregateRootId = aggregateRootId,
                DateCreated = DateTime.UtcNow,
                SequenceStart = firstEvent.Sequence,
                SequenceEnd = lastEvent.Sequence,
                EventData = events
            };

            var aggregateRootDirectory = Path.Combine(FileUtility.GetEventDirectory(), aggregateRootId.ToString());

            if (!Directory.Exists(aggregateRootDirectory))
            {
                Directory.CreateDirectory(aggregateRootDirectory);
            }

            var fileName = string.Format("{0}-{1}.{2}", firstEvent.Sequence, lastEvent.Sequence, extension);
            var eventPath = Path.Combine(aggregateRootDirectory, fileName);

            var serialized = GetEventStreamText(stream);
            File.WriteAllText(eventPath, serialized);
        }
    }
}