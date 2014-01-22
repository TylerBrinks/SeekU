using System;
using Newtonsoft.Json;
using SeekU.Domain;
using SeekU.Eventing;

namespace SeekU.Sql.Eventing
{
    /// <summary>
    /// SQL snapshot storage provider
    /// </summary>
    public class SqlSnapshotStore : ISnapshotStore
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        public Func<ISqlDataStore> GetDatabase = () => new SqlDataStore();

        /// <summary>
        /// Globally sets the name of the snapshot connection string for SQL 
        /// </summary>
        public string ConnectionStringName
        {
            get { return SqlDataStore.SnapshotConnectionStringName; }
            set { SqlDataStore.SnapshotConnectionStringName = value; }
        }

        /// <summary>
        /// Gets a snapsnot for a given ID if one exists
        /// </summary>
        /// <typeparam name="T">Type of snapshot detail</typeparam>
        /// <param name="aggregateRootId">ID of the aggregate the snapshot was taken from</param>
        /// <returns>Snapshot instance</returns>
        public Snapshot<T> GetSnapshot<T>(Guid aggregateRootId)
        {
            var detail = GetDatabase().GetSnapshot(aggregateRootId);

            if (detail == null)
            {
                return null;
            }

            return new Snapshot<T>
            {
                AggregateRootId = detail.AggregateRootId,
                Version = detail.Version,
                Data = JsonConvert.DeserializeObject<T>(detail.SnapshotData, SerializerSettings)
            };
        }

        /// <summary>
        /// Saves or updates the current snapshot for a given aggregate
        /// </summary>
        /// <typeparam name="T">Type of snapshot detail</typeparam>
        /// <param name="snapshot">Snapshot instance</param>
        public void SaveSnapshot<T>(Snapshot<T> snapshot)
        {
            var snapshotDetail = new SnapshotDetail
            {
                AggregateRootId = snapshot.AggregateRootId,
                Version = snapshot.Version,
                SnapshotData = JsonConvert.SerializeObject(snapshot.Data, SerializerSettings)
            };

            GetDatabase().InsertSnapshot(snapshotDetail);
        }
    }
}
