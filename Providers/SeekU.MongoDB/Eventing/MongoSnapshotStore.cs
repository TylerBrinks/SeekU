using System;
using SeekU.Domain;
using SeekU.Eventing;

namespace SeekU.MongoDB.Eventing
{
    /// <summary>
    /// MongoDB snapshot storage provider
    /// </summary>
    public class MongoSnapshotStore : ISnapshotStore
    {
        public Func<IMongoRepository> GetRepository = () => new MongoRepository();

        /// <summary>
        /// Globally sets the name of the snapshot connection string for MongoDB 
        /// </summary>
        public string ConnectionStringName
        {
            get { return MongoRepository.SnapshotConnectionStringName; }
            set { MongoRepository.SnapshotConnectionStringName = value; }
        }

        /// <summary>
        /// Globally sets the name of the snapshot database in MongoDB
        /// </summary>
        public string DatabaseName
        {
            get { return MongoRepository.SnapshotDatabaseName; }
            set { MongoRepository.SnapshotDatabaseName = value; }
        }

        /// <summary>
        /// Gets a snapsnot for a given ID if one exists
        /// </summary>
        /// <typeparam name="T">Type of snapshot detail</typeparam>
        /// <param name="aggregateRootId">Id of the aggregate the snapshot was taken from</param>
        /// <returns>Snapshot instance</returns>
        public Snapshot<T> GetSnapshot<T>(Guid aggregateRootId)
        {
            var detail = GetRepository().GetSnapshot(aggregateRootId);

            if (detail == null)
            {
                return null;
            }

            return new Snapshot<T>
            {
                AggregateRootId = detail.AggregateRootId,
                Version = detail.Version,
                Data = (T)detail.SnapshotData
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
                SnapshotData = snapshot.Data
            };

            GetRepository().InsertSnapshot(snapshotDetail);
        }
    }
}
