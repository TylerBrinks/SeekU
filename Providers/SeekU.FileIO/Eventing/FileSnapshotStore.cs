using System.IO;
using SeekU.Domain;

namespace SeekU.FileIO.Eventing
{
    public abstract class FileSnapshotStore
    {
        public abstract SnapshotDetail GetSnapshotDetail(string filePath);

        public abstract string GetSnapshotText(SnapshotDetail snapshotData);

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