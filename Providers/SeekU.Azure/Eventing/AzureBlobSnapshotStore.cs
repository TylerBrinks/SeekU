using System;
using SeekU.Domain;
using SeekU.Eventing;

namespace SeekU.Azure.Eventing
{
    /// <summary>
    /// Azure blob snapshot storage provider
    /// </summary>
    public class AzureBlobSnapshotStore : ISnapshotStore
    {
        public Func<IAzureStorageRepository> GetRepository = () => new AzureStorageRepository();

        /// <summary>
        /// Globally sets the name of the Azure snapshot connection string 
        /// </summary>
        public string ConnectionString
        {
            get { return AzureStorageRepository.SnapshotConnectionString; }
            set { AzureStorageRepository.SnapshotConnectionString = value; }
        }

        /// <summary>
        /// Globally sets the name of the Azure snapshot container name
        /// </summary>
        public string ContainerName
        {
            get { return AzureStorageRepository.SnapshotContainerName; }
            set { AzureStorageRepository.SnapshotContainerName = value; }
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
