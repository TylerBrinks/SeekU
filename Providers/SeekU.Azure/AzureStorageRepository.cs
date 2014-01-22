using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
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
        //private static bool _eventContainerCreated;
        private static bool _snapshotContainerCreated;
        private static string _snapshotContainerName = "snapshots";
        //private static string _eventContainerName = "eventstream";

        //internal static string EventConnectionString { get; set; }

        internal static string SnapshotConnectionString{ get; set; }

        //internal static string EventContainerName
        //{
        //    get { return _eventContainerName; }
        //    set { _eventContainerName = value; }
        //}

        internal static string SnapshotContainerName
        {
            get { return _snapshotContainerName; }
            set { _snapshotContainerName = value; }
        }
        #endregion

        ///// <summary>
        ///// Gets an event stream from Azure for a given ID
        ///// </summary>
        ///// <param name="aggregateRoodId">Aggregate root ID</param>
        ///// <param name="startVersion">Starting version of the event stream</param>
        ///// <returns>List of events</returns>
        //public List<EventStream> GetEventStream(Guid aggregateRoodId, long startVersion)
        //{
        //    CreateContainers();

        //    var container = GetContainer(EventConnectionString, _eventContainerName);

        //    var blob = container.GetBlockBlobReference(string.Format("{0}-{1}", aggregateRoodId, startVersion));

        //    return null;
        //}

        ///// <summary>
        ///// Inserts a new event stream
        ///// </summary>
        ///// <param name="events">Events to insert</param>
        //public void InsertEvents(EventStream events)
        //{
        //    CreateContainers();

        //    //var collection = GetRepository(_eventConnectionStringName).GetCollection<EventStream>("EventStream");
        //    //collection.Insert(events);

            
        //}

        /// <summary>
        /// Gets a snapshot for a give ID if one exists
        /// </summary>
        /// <param name="aggregateRootId">Aggregate root ID</param>
        /// <returns>Snapshot details</returns>
        public SnapshotDetail GetSnapshot(Guid aggregateRootId)
        {
            CreateContainers();

            var container = GetContainer(SnapshotContainerName, SnapshotConnectionString);
            var blob = container.GetBlockBlobReference(aggregateRootId.ToString().ToLower());

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
            CreateContainers();

            var container = GetContainer(SnapshotContainerName, SnapshotConnectionString);
            var blob = container.GetBlockBlobReference(snapshot.AggregateRootId.ToString().ToLower());

            var blobText = JsonConvert.SerializeObject(snapshot, SerializerSettings);

            blob.UploadText(blobText);
        }

        /// <summary>
        /// Gets a reference to an Azure blob container
        /// </summary>
        /// <param name="containerName">Name of the container</param>
        /// <param name="connectionString">Azure blob connection string</param>
        /// <returns>Azure blob container</returns>
        private static CloudBlobContainer GetContainer( string containerName, string connectionString)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();
            return client.GetContainerReference(containerName);
        }

        /// <summary>
        /// Creates the event stream and snapshot blob containers if they don't exist
        /// </summary>
        private static void CreateContainers()
        {
            lock (Sync)
            {
                //if (!_eventContainerCreated)
                //{
                //    _eventContainerCreated = true;
                //    CreateContainer(_eventContainerName, EventConnectionString);
                //}

                if (!_snapshotContainerCreated)
                {
                    _snapshotContainerCreated = true;
                    CreateContainer(_snapshotContainerName, SnapshotConnectionString);
                }
            }
        }

        /// <summary>
        /// Create a container if it does not exist
        /// </summary>
        /// <param name="containerName">Name of the container</param>
        /// <param name="connectionString">Azure blob connection string</param>
        private static void CreateContainer(string containerName, string connectionString)
        {
            GetContainer(containerName, connectionString).CreateIfNotExists();
        }
    }
}
