using System;
using System.Collections.Generic;

namespace SeekU.Sql
{
    /// <summary>
    /// Represents event stream and snapshot persistance for MongoDB
    /// </summary>
    public class SqlRepository : ISqlRepository
    {
        #region Sql statements

        private const string EventStreamTableName = "EventStream";
        private const string SnapshotTableName = "Snapshots";
        private const string GetEventsForId = "select * from " + EventStreamTableName + " where AggregateRootId = @0 and SequenceStart >= @1 order by SequenceStart";
        private const string TopOneSnapshot = "select top 1 * from  " + SnapshotTableName + " where AggregateRootId = @0 order by Version desc";
        private const string TableExists = "select count(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = @0";
        private const string CreateEventStreamTable = "create table " + EventStreamTableName + " ([Id] [bigint] IDENTITY(1,1) NOT NULL,[SequenceStart] [bigint] NOT NULL,[SequenceEnd] [bigint] NOT NULL,[AggregateRootId] [uniqueidentifier] NOT NULL,[DateCreated] [datetime] NOT NULL,[EventData] [varchar](max) NULL,PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) )";
        private const string CreateSnapshotsTable = "create table  " + SnapshotTableName + " ([Id] [bigint] IDENTITY(1,1) NOT NULL,[AggregateRootId] [uniqueidentifier] NOT NULL,[Version] [bigint] NOT NULL,[SnapshotData] [varchar](max) NULL,PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON))";
        #endregion

        #region Defaults
        // Default connection string name
        private static string _eventConnectionStringName = "SeekU.Sql";
        private static string _snapshotConnectionStringName = "SeekU.Sql";
        private static readonly object Sync = new object();
        private static bool _eventTableCreated;
        private static bool _snapshotTableCreated;

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
        #endregion

        /// <summary>
        /// Gets an event stream from SQL for a given ID
        /// </summary>
        /// <param name="aggregateRootId">Aggregate root ID</param>
        /// <param name="startVersion">Starting version of the event stream</param>
        /// <returns>List of events</returns>
        public List<EventStream> GetEventStream(Guid aggregateRootId, long startVersion)
        {
            CreateEventTable();

            using (var db = new PetaPoco.Database(_eventConnectionStringName))
            {
                var events = db.Fetch<EventStream>(GetEventsForId, aggregateRootId, startVersion);

                return events;
            }
        }

        /// <summary>
        /// Inserts a new event stream
        /// </summary>
        /// <param name="events">Events to insert</param>
        public void InsertEvents(EventStream events)
        {
            CreateEventTable();
            
            using (var db = new PetaPoco.Database(_eventConnectionStringName))
            {
                db.Insert(EventStreamTableName, "Id", events);
            }
        }

        /// <summary>
        /// Gets a snapshot for a give ID if one exists
        /// </summary>
        /// <param name="aggregateRootId">Aggregate root ID</param>
        /// <returns>Snapshot details</returns>
        public SnapshotDetail GetSnapshot(Guid aggregateRootId)
        {
            CreateSnapshotTable();

            using (var db = new PetaPoco.Database(_snapshotConnectionStringName))
            {
                return db.SingleOrDefault<SnapshotDetail>(TopOneSnapshot, aggregateRootId);
            }
        }

        /// <summary>
        /// Inserts a new snapshot
        /// </summary>
        /// <param name="snapshot">Snapshot instance</param>
        public void InsertSnapshot(SnapshotDetail snapshot)
        {
            CreateSnapshotTable();

            using (var db = new PetaPoco.Database(_snapshotConnectionStringName))
            {
                db.Insert(SnapshotTableName, "Id", snapshot);
            }
        }

        /// <summary>
        /// Creates the event stream table if it doesn't exist
        /// </summary>
        private static void CreateEventTable()
        {
            lock (Sync)
            {
                if (_eventTableCreated)
                {
                    return;
                }

                _eventTableCreated = true;
                CreateTable(EventStreamTableName, CreateEventStreamTable, _eventConnectionStringName);
            }
        }

        /// <summary>
        /// Creates the snapshot table if it doesn't exist
        /// </summary>
        private static void CreateSnapshotTable()
        {
            lock (Sync)
            {
                if (_snapshotTableCreated)
                {
                    return;
                }

                _snapshotTableCreated = true;
                CreateTable(SnapshotTableName, CreateSnapshotsTable, _snapshotConnectionStringName);
            }
        }

        /// <summary>
        /// Creates tables if they don't already exist in the database
        /// </summary>
        /// <param name="tableName">Name of the table to create</param>
        /// <param name="sql">SQL statement for table creation</param>
        /// <param name="connectionString">SQL connection string</param>
        private static void CreateTable(string tableName, string sql, string connectionString)
        {
            using (var db = new PetaPoco.Database(connectionString))
            {
                var count = db.ExecuteScalar<int>(TableExists, tableName);

                if (count > 0)
                {
                    return;
                }

                db.Execute(sql);
            }
        }
    }
}
