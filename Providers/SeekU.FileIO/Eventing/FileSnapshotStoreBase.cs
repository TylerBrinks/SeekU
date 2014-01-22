using System.IO;
using SeekU.Domain;

namespace SeekU.FileIO.Eventing
{
    // <summary>
    /// Base class for file-base snapshot storage
    /// </summary>
    public abstract class FileSnapshotStoreBase
    {
        /// <summary>
        /// Gets snapshot details from disk
        /// </summary>
        /// <param name="filePath">Path to the snapshot file</param>
        /// <returns>Snapshot details</returns>
        public abstract SnapshotDetail GetSnapshotDetail(string filePath);

        /// <summary>
        /// Gets the text to save as the file's contents
        /// </summary>
        /// <param name="snapshotData">Snapshot details</param>
        /// <returns>Text representation of the snapshot details</returns>
        public abstract string GetSnapshotText(SnapshotDetail snapshotData);

        /// <summary>
        /// Gets snapshot details from disk
        /// </summary>
        /// <typeparam name="T">Type of snapshot</typeparam>
        /// <param name="filePath">Path to the snapshot file</param>
        /// <returns>Snapshot instance</returns>
        public Snapshot<T> GetSnapshotInstance<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            var detail = GetSnapshotDetail(filePath);

            return new Snapshot<T>
            {
                AggregateRootId = detail.AggregateRootId,
                Version = detail.Version,
                Data = (T)detail.Data
            };
        }

        /// <summary>
        /// Saves a snapshot to a given file
        /// </summary>
        /// <typeparam name="T">Type of snapshot</typeparam>
        /// <param name="snapshot">Snapshot instance</param>
        /// <param name="fileName">File name to create or overwrite</param>
        public void SaveSnapshotInstance<T>(Snapshot<T> snapshot, string fileName)
        {
            var aggregateRootDirectory = Path.Combine(FileUtility.GetSnapshotDirectory(),
                snapshot.AggregateRootId.ToString());

            if (!Directory.Exists(aggregateRootDirectory))
            {
                Directory.CreateDirectory(aggregateRootDirectory);
            }

            var snapshotData = new SnapshotDetail
            {
                AggregateRootId = snapshot.AggregateRootId,
                Version = snapshot.Version,
                Data = snapshot.Data
            };

            var filePath = Path.Combine(aggregateRootDirectory, fileName);
            File.WriteAllText(filePath, GetSnapshotText(snapshotData));
        }
    }
}