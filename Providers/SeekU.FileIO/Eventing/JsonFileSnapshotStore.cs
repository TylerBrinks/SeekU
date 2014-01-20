using System;
using System.IO;
using Newtonsoft.Json;
using SeekU.Domain;
using SeekU.Eventing;

namespace SeekU.FileIO.Eventing
{
    public class JsonFileSnapshotStore : FileSnapshotStore, ISnapshotStore
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

        public override SnapshotDetail GetSnapshotDetail(string filePath)
        {
            var fileText = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<SnapshotDetail>(fileText, SerializerSettings);
        }

        public override string GetSnapshotText(SnapshotDetail snapshotData)
        {
            return JsonConvert.SerializeObject(snapshotData, SerializerSettings);
        }

        public Snapshot<T> GetSnapshot<T>(Guid aggregateRootId)
        {
            var filePath = Path.Combine(FileUtility.GetSnapshotDirectory(), aggregateRootId.ToString(), FileName);

            return GetSnapshotInstance<T>(filePath);
        }

        public void SaveSnapshot<T>(Snapshot<T> snapshot)
        {
            SaveSnapshotInstance(snapshot, FileName);
        }

        public string FileName { get; set; }
    }
}
