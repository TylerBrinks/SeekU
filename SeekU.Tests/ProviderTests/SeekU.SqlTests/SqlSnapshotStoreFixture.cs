using System;
using Moq;
using NUnit.Framework;
using SampleDomain.Domain;
using SeekU.Domain;
using SeekU.Sql;
using SeekU.Sql.Eventing;
using SeekU.Tests.Properties;

namespace SeekU.Tests.ProviderTests.SeekU.SqlTests
{
    [TestFixture]
    public class SqlSnapshotStoreFixture
    {
        [Test]
        public void SqlSnapshotStore_Returns_Null_When_No_Snapshot_Exists()
        {
            var database = new Mock<ISqlRepository>();
            database.Setup(db => db.GetSnapshot(It.IsAny<Guid>()));

            var store = new SqlSnapshotStore
            {
                GetRepository = () => database.Object
            };

            var snapshot = store.GetSnapshot<BankAccountSnapshot>(Guid.NewGuid());

            Assert.IsNull(snapshot);
        }

        [Test]
        public void SqlSnapshotStore_Deserializes_Snapshot_Details()
        {
            var database = new Mock<ISqlRepository>();
            database.Setup(db => db.GetSnapshot(It.IsAny<Guid>())).Returns(new SnapshotDetail{SnapshotData = Resources.JsonSerializedSnapshot});

            var store = new SqlSnapshotStore
            {
                GetRepository = () => database.Object
            };

            var snapshot = store.GetSnapshot<BankAccountSnapshot>(Guid.NewGuid());

            Assert.AreEqual(900, snapshot.Data.Balance);
        }

        [Test]
        public void SqlSnapshotStore_Serializes_Snapshots()
        {
            var database = new Mock<ISqlRepository>();
            database.Setup(db => db.InsertSnapshot(It.IsAny<SnapshotDetail>()));

            var store = new SqlSnapshotStore
            {
                GetRepository = () => database.Object
            };

            var snapshot = new Snapshot<BankAccountSnapshot> {Data = new BankAccountSnapshot {Balance = 900}};
            store.SaveSnapshot(snapshot);

            database.Verify(db => db.InsertSnapshot(It.IsAny<SnapshotDetail>()), Times.Once());
        }
    }
}
