using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace SeekU.MongoDB
{
    /// <summary>
    /// MongoDB event stream and snapshot storage implementation
    /// </summary>
    public class MongoRepository : IMongoRepository
    {
        #region Defaults
        private static string _eventConnectionStringName = "SeekU.MongoDB.ConnectionStringName";
        private static string _snapshotConnectionStringName = "SeekU.MongoDB.ConnectionStringName";
        private static string _snapshotDatabaseName = "SeekU";
        private static string _eventDatabaseName = "SeekU";

        internal static string EventConnectionStringName
        {
            get { return _eventConnectionStringName; }
            set { _eventConnectionStringName = value; }
        }

        internal static string SnapshotConnectionStringName
        {
            get { return _snapshotConnectionStringName; }
            set { _snapshotConnectionStringName = value; }
        }

        internal static string EventDatabaseName
        {
            get { return _eventDatabaseName; }
            set { _eventDatabaseName = value; }
        }

        internal static string SnapshotDatabaseName
        {
            get { return _snapshotDatabaseName; }
            set { _snapshotDatabaseName = value; }
        }
        #endregion

        /// <summary>
        /// Gets an event stream from MongoDB for a given ID
        /// </summary>
        /// <param name="aggregateRoodId">Aggregate root ID</param>
        /// <param name="startVersion">Starting version of the event stream</param>
        /// <returns>List of events</returns>
        public List<EventStream> GetEventStream(Guid aggregateRoodId, long startVersion)
        {
            var collection = GetDatabase(_eventConnectionStringName, _eventDatabaseName).GetCollection<EventStream>("EventStream");

            var query = collection
                .AsQueryable()
                .Where(e => e.AggregateRootId == aggregateRoodId && e.SequenceStart >= startVersion);

            return query.ToList();
        }

        /// <summary>
        /// Inserts a new event stream
        /// </summary>
        /// <param name="events">Events to insert</param>
        public void InsertEvents(EventStream events)
        {
            var collection = GetDatabase(_eventConnectionStringName, _eventDatabaseName).GetCollection<EventStream>("EventStream");
            collection.Insert(events);
        }

        /// <summary>
        /// Gets a snapshot for a give ID if one exists
        /// </summary>
        /// <param name="aggregateRootId">Aggregate root ID</param>
        /// <returns>Snapshot details</returns>
        public SnapshotDetail GetSnapshot(Guid aggregateRootId)
        {
            var collection = GetDatabase(_snapshotConnectionStringName, _snapshotDatabaseName).GetCollection<SnapshotDetail>("Snapshots");

            return collection.FindOne(Query<SnapshotDetail>.EQ(s => s.AggregateRootId, aggregateRootId));
        }

        /// <summary>
        /// Inserts a new snapshot
        /// </summary>
        /// <param name="snapshot">Snapshot instance</param>
        public void InsertSnapshot(SnapshotDetail snapshot)
        {
            var existing = GetSnapshot(snapshot.AggregateRootId);
            var collection = GetDatabase(_snapshotConnectionStringName, _snapshotDatabaseName).GetCollection<SnapshotDetail>("Snapshots");

            if (existing != null)
            {
                snapshot.Id = existing.Id;
            }

            collection.Save(snapshot);
        }

        private static MongoDatabase GetDatabase(string connectionName, string databaseName)
        {
            var connectoinString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            var client = new MongoClient(connectoinString);

            var server = client.GetServer();
            return server.GetDatabase(databaseName);
        }
    }
}
