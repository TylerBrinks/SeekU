using System;
using Newtonsoft.Json;
using SeekU.Domain;
using SeekU.Eventing;

namespace SeekU.Sql.Eventing
{
    public class SqlSnapshotStore : ISnapshotStore
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        public Func<ISqlDatabase> GetDatabase = () => new SqlDatabase(); 

        public string ConnectionStringName
        {
            get { return SqlDatabase.SnapshotConnectionStringName; }
            set { SqlDatabase.SnapshotConnectionStringName = value; }
        }

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

        public void SaveSnapshot<T>(Snapshot<T> snapshot)
        {
            var snapshotData = new SnapshotDetail
            {
                AggregateRootId = snapshot.AggregateRootId,
                Version = snapshot.Version,
                SnapshotData = JsonConvert.SerializeObject(snapshot.Data, SerializerSettings)
            };

            GetDatabase().InsertSnapshot(snapshotData);
        }
    }
}
