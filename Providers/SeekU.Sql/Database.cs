using System;
using System.Collections.Generic;
using System.Configuration;

namespace SeekU.Sql
{
    internal class Database
    {
        #region Sql statements
        private const string GetEventsForId = "select * from EventStream where AggregateRootId = @0 and SequenceStart >= @1";
        private const string TopOneSnapshot = "select top 1 * from Snapshots where AggregateRootId = @0 order by Version desc";
        private const string TableExists = "select count(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = @0";
        private const string CreateEventStreamTable = "create table EventStream ([Id] [bigint] IDENTITY(1,1) NOT NULL,[SequenceStart] [bigint] NOT NULL,[SequenceEnd] [bigint] NOT NULL,[AggregateRootId] [uniqueidentifier] NOT NULL,[DateCreated] [datetime] NOT NULL,[EventData] [varchar](max) NULL,PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
        private const string CreateSnapshotsTable = "create table Snapshots ([Id] [bigint] IDENTITY(1,1) NOT NULL,[AggregateRootId] [uniqueidentifier] NOT NULL,[Version] [bigint] NOT NULL,[SnapshotData] [varchar](max) NULL,PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
        #endregion

        // Default connection string name
        private static readonly string ConnectionStringName = "SeekU";

        static Database()
        {
            if (ConfigurationManager.AppSettings["SeekU.SqlConnectionName"] != null)
            {
                ConnectionStringName = ConfigurationManager.AppSettings["SeekU.SqlConnectionName"];
            }

            CreateTable("EventStream", CreateEventStreamTable);
            CreateTable("Snapshots", CreateSnapshotsTable);
        }

        internal static List<EventStream> GetEventStream(Guid aggregateRootId, long startVersion)
        {
            using (var db = new PetaPoco.Database(ConnectionStringName))
            {
                var events = db.Fetch<EventStream>(GetEventsForId, aggregateRootId, startVersion);

                return events;
            }
        }

        internal static void InsertEvents(EventStream events)
        {
            using (var db = new PetaPoco.Database(ConnectionStringName))
            {
                db.Insert("EventStream", "Id", events);
            }
        }

        internal static SnapshotDetail GetSnapshot(Guid aggregateRootId)
        {
            using (var db = new PetaPoco.Database(ConnectionStringName))
            {
                return db.SingleOrDefault<SnapshotDetail>(TopOneSnapshot, aggregateRootId);
            }
        }

        internal static void InsertSnapshot(SnapshotDetail snapshot)
        {
            using (var db = new PetaPoco.Database(ConnectionStringName))
            {
                db.Insert("Snapshots", "Id", snapshot);
            }
        }

        internal static void CreateTable(string tableName, string sql)
        {
            using (var db = new PetaPoco.Database(ConnectionStringName))
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
