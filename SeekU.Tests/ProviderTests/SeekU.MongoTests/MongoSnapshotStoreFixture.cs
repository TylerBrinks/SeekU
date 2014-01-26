using System;
using Moq;
using NUnit.Framework;
using SampleDomain.Domain;
using SeekU.Domain;
using SeekU.MongoDB;
using SeekU.MongoDB.Eventing;

namespace SeekU.Tests.ProviderTests.SeekU.MongoTests
{
    [TestFixture]
    public class MongoSnapshotStoreFixture
    {
        [Test]
        public void MongoSnapshotStore_Returns_Null_When_No_Snapshot_Exists()
        {
            var database = new Mock<IMongoRepository>();
            database.Setup(db => db.GetSnapshot(It.IsAny<Guid>()));

            var store = new MongoSnapshotStore
            {
                GetRepository = () => database.Object
            };

            var snapshot = store.GetSnapshot<BankAccountSnapshot>(Guid.NewGuid());

            Assert.IsNull(snapshot);
        }

        [Test]
        public void MongoSnapshotStore_Deserializes_Snapshot_Details()
        {
            var database = new Mock<IMongoRepository>();
            database.Setup(db => db.GetSnapshot(It.IsAny<Guid>())).Returns(new SnapshotDetail{SnapshotData = new BankAccountSnapshot{Balance = 900}});

            var store = new MongoSnapshotStore
            {
                GetRepository = () => database.Object
            };

            var snapshot = store.GetSnapshot<BankAccountSnapshot>(Guid.NewGuid());

            Assert.AreEqual(900, snapshot.Data.Balance);
        }

        [Test]
        public void MongoSnapshotStore_Serializes_Snapshots()
        {
            var database = new Mock<IMongoRepository>();
            database.Setup(db => db.InsertSnapshot(It.IsAny<SnapshotDetail>()));

            var store = new MongoSnapshotStore
            {
                GetRepository = () => database.Object
            };

            var snapshot = new Snapshot<BankAccountSnapshot> {Data = new BankAccountSnapshot {Balance = 900}};
            store.SaveSnapshot(snapshot);

            database.Verify(db => db.InsertSnapshot(It.IsAny<SnapshotDetail>()), Times.Once());
        }
    }
}
