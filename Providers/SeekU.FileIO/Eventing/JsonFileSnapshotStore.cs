using System;
using System.IO;
using Newtonsoft.Json;
using SeekU.Domain;
using SeekU.Eventing;

namespace SeekU.FileIO.Eventing
{
    /// <summary>
    /// Saves snapshots to files serialized in json format
    /// </summary>
    public class JsonFileSnapshotStore : FileSnapshotStoreBase, ISnapshotStore
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        public JsonFileSnapshotStore()
        {
            FileName = "snapshot.json";   
        }

        /// <summary>
        /// Gets snapshot details from disk
        /// </summary>
        /// <param name="filePath">Path to the snapshot file</param>
        /// <returns>Snapshot details</returns>
        public override SnapshotDetail GetSnapshotDetail(string filePath)
        {
            var fileText = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<SnapshotDetail>(fileText, SerializerSettings);
        }

        /// <summary>
        /// Gets the text to save as the file's contents
        /// </summary>
        /// <param name="snapshotData">Snapshot details</param>
        /// <returns>Text representation of the snapshot details</returns>
        public override string GetSnapshotText(SnapshotDetail snapshotData)
        {
            return JsonConvert.SerializeObject(snapshotData, SerializerSettings);
        }

        /// <summary>
        /// Gets a snapshot for a give ID.
        /// </summary>
        /// <typeparam name="T">Type of snapshot</typeparam>
        /// <param name="aggregateRootId">Aggregate root ID</param>
        /// <returns>Snapshot instance</returns>
        public Snapshot<T> GetSnapshot<T>(Guid aggregateRootId)
        {
            var filePath = Path.Combine(FileUtility.GetSnapshotDirectory(), aggregateRootId.ToString(), FileName);

            return GetSnapshotInstance<T>(filePath);
        }

        /// <summary>
        /// Creates or overwrites a snapshot to disk
        /// </summary>
        /// <typeparam name="T">Type of snapshot</typeparam>
        /// <param name="snapshot">Snapshot instance</param>
        public void SaveSnapshot<T>(Snapshot<T> snapshot)
        {
            SaveSnapshotInstance(snapshot, FileName);
        }

        public string FileName { get; set; }
    }
}
