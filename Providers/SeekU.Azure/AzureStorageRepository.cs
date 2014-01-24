using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace SeekU.Azure
{
    /// <summary>
    /// Azure event stream and snapshot storage implementation
    /// </summary>
    public class AzureStorageRepository : IAzureStorageRepository
    {
        /// <summary>
        /// Serializer settings to preserve namespaces
        /// </summary>
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        #region Defaults
        private static readonly object Sync = new object();
        private static bool _eventTableCreated;
        private static bool _snapshotContainerCreated;
        private static string _snapshotContainerName = "snapshots";
        private static string _eventTableName = "eventstream";

        internal static string EventConnectionString { get; set; }

        internal static string SnapshotConnectionString{ get; set; }

        internal static string EventTableName
        {
            get { return _eventTableName; }
            set { _eventTableName = value; }
        }

        internal static string SnapshotContainerName
        {
            get { return _snapshotContainerName; }
            set { _snapshotContainerName = value; }
        }
        #endregion

        /// <summary>
        /// Gets an event stream from Azure for a given ID
        /// </summary>
        /// <param name="aggregateRoodId">Aggregate root ID</param>
        /// <param name="startVersion">Starting version of the event stream</param>
        /// <returns>List of events</returns>
        public List<EventStream> GetEventStream(Guid aggregateRoodId, long startVersion)
        {
            CreateEventContainer();

            // Get all rows with the partition key == aggregate root and row key >= start version
            var query = new TableQuery<EventStream>()
                .Where(TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,aggregateRoodId.ToString()),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual,startVersion.ToString(CultureInfo.InvariantCulture))
                    ));

            var results = GetTable().ExecuteQuery(query);

            return results.ToList();
        }

        /// <summary>
        /// Inserts a new event stream
        /// </summary>
        /// <param name="events">Events to insert</param>
        public void InsertEvents(EventStream events)
        {
            CreateEventContainer();

            var table = GetTable();
            table.Execute(TableOperation.Insert(events));
        }

        /// <summary>
        /// Gets a snapshot for a give ID if one exists
        /// </summary>
        /// <param name="aggregateRootId">Aggregate root ID</param>
        /// <returns>Snapshot details</returns>
        public SnapshotDetail GetSnapshot(Guid aggregateRootId)
        {
            CreateSnapshotContainer();

            var blob = GetContainer().GetBlockBlobReference(aggregateRootId.ToString().ToLower());

            if (!blob.Exists())
            {
                return null;
            }

            var blobText = blob.DownloadText();

            return JsonConvert.DeserializeObject<SnapshotDetail>(blobText, SerializerSettings);
        }

        /// <summary>
        /// Inserts a new snapshot
        /// </summary>
        /// <param name="snapshot">Snapshot instance</param>
        public void InsertSnapshot(SnapshotDetail snapshot)
        {
            CreateSnapshotContainer();

            var blob = GetContainer().GetBlockBlobReference(snapshot.AggregateRootId.ToString().ToLower());

            var blobText = JsonConvert.SerializeObject(snapshot, SerializerSettings);

            blob.UploadText(blobText);
        }

        /// <summary>
        /// Gets a reference to an Azure blob container
        /// </summary>
        /// <returns>Azure blob container</returns>
        private static CloudBlobContainer GetContainer()
        {
            var account = CloudStorageAccount.Parse(SnapshotConnectionString);
            var client = account.CreateCloudBlobClient();
            return client.GetContainerReference(_snapshotContainerName);
        }
       
        /// <summary>
        /// Gets a reference to an Azure table
        /// </summary>
        /// <returns>Azure table</returns>
        private static CloudTable GetTable()
        {
            var account = CloudStorageAccount.Parse(EventConnectionString);
            var client = account.CreateCloudTableClient();
            return client.GetTableReference(_eventTableName);
        }

        /// <summary>
        /// Creates the event stream table if ot doesn't exist
        /// </summary>
        private static void CreateEventContainer()
        {
            lock (Sync)
            {
                if (_eventTableCreated)
                {
                    return;
                }

                _eventTableCreated = true;
                GetTable().CreateIfNotExists();
            }
        }

        /// <summary>
        /// Creates the snapshot blob containers if it doesn't exist
        /// </summary>
        private static void CreateSnapshotContainer()
        {
            lock (Sync)
            {
                if (_snapshotContainerCreated)
                {
                    return;
                }

                _snapshotContainerCreated = true;
                GetContainer().CreateIfNotExists();
            }
        }
    }
}
