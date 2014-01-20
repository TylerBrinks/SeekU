using System;
using System.Collections.Generic;
using System.Configuration;

namespace SeekU.Sql
{
    internal class Database
    {
        #region Sql statements

        private const string EventStreamTableName = "EventStream";
        private const string SnapshotTableName = "Snapshots";
        private const string GetEventsForId = "select * from " + EventStreamTableName + " where AggregateRootId = @0 and SequenceStart >= @1";
        private const string TopOneSnapshot = "select top 1 * from  " + SnapshotTableName + " where AggregateRootId = @0 order by Version desc";
        private const string TableExists = "select count(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = @0";
        private const string CreateEventStreamTable = "create table " + EventStreamTableName + " ([Id] [bigint] IDENTITY(1,1) NOT NULL,[SequenceStart] [bigint] NOT NULL,[SequenceEnd] [bigint] NOT NULL,[AggregateRootId] [uniqueidentifier] NOT NULL,[DateCreated] [datetime] NOT NULL,[EventData] [varchar](max) NULL,PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
        private const string CreateSnapshotsTable = "create table  " + SnapshotTableName + " ([Id] [bigint] IDENTITY(1,1) NOT NULL,[AggregateRootId] [uniqueidentifier] NOT NULL,[Version] [bigint] NOT NULL,[SnapshotData] [varchar](max) NULL,PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
        #endregion

        // Default connection string name
        private static string _eventConnectionStringName = "SeekU";
        private static string _snapshotConnectionStringName = "SeekU";
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

        internal static List<EventStream> GetEventStream(Guid aggregateRootId, long startVersion)
        {
            CreateTables();

            using (var db = new PetaPoco.Database(_eventConnectionStringName))
            {
                var events = db.Fetch<EventStream>(GetEventsForId, aggregateRootId, startVersion);

                return events;
            }
        }

        internal static void InsertEvents(EventStream events)
        {
            CreateTables();
            
            using (var db = new PetaPoco.Database(_eventConnectionStringName))
            {
                db.Insert(EventStreamTableName, "Id", events);
            }
        }

        internal static SnapshotDetail GetSnapshot(Guid aggregateRootId)
        {
            CreateTables();

            using (var db = new PetaPoco.Database(_snapshotConnectionStringName))
            {
                return db.SingleOrDefault<SnapshotDetail>(TopOneSnapshot, aggregateRootId);
            }
        }

        internal static void InsertSnapshot(SnapshotDetail snapshot)
        {
            CreateTables();

            using (var db = new PetaPoco.Database(_snapshotConnectionStringName))
            {
                db.Insert(SnapshotTableName, "Id", snapshot);
            }
        }

        private static void CreateTables()
        {
            lock (Sync)
            {
                if (!_eventTableCreated)
                {
                    _eventTableCreated = true;
                    CreateTable(EventStreamTableName, CreateEventStreamTable, _eventConnectionStringName);
                }

                if (!_snapshotTableCreated)
                {
                    _snapshotTableCreated = true;
                    CreateTable(SnapshotTableName, CreateSnapshotsTable, _snapshotConnectionStringName);
                }
            }
        }

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
