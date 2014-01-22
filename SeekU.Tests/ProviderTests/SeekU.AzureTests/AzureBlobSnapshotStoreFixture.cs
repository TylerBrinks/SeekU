using System;
using Moq;
using NUnit.Framework;
using SampleDomain.Domain;
using SeekU.Azure;
using SeekU.Azure.Eventing;
using SeekU.Domain;

namespace SeekU.Tests.ProviderTests.SeekU.AzureTests
{
    [TestFixture]
    public class AzureBlobSnapshotStoreFixture
    {
        [Test]
        public void AzureSnapshotStore_Returns_Null_When_No_Snapshot_Exists()
        {
            var database = new Mock<IAzureStorageRepository>();
            database.Setup(db => db.GetSnapshot(It.IsAny<Guid>()));

            var store = new AzureBlobSnapshotStore
            {
                GetRepository = () => database.Object
            };

            var snapshot = store.GetSnapshot<BankAccountSnapshot>(Guid.NewGuid());

            Assert.IsNull(snapshot);
        }

        [Test]
        public void AzureSnapshotStore_Deserializes_Snapshot_Details()
        {
            var database = new Mock<IAzureStorageRepository>();
            database.Setup(db => db.GetSnapshot(It.IsAny<Guid>())).Returns(new SnapshotDetail{SnapshotData = new BankAccountSnapshot{Balance = 900}});

            var store = new AzureBlobSnapshotStore
            {
                GetRepository = () => database.Object
            };

            var snapshot = store.GetSnapshot<BankAccountSnapshot>(Guid.NewGuid());

            Assert.AreEqual(900, snapshot.Data.Balance);
        }

        [Test]
        public void AzureSnapshotStore_Serializes_Snapshots()
        {
            var database = new Mock<IAzureStorageRepository>();
            database.Setup(db => db.InsertSnapshot(It.IsAny<SnapshotDetail>()));

            var store = new AzureBlobSnapshotStore
            {
                GetRepository = () => database.Object
            };

            var snapshot = new Snapshot<BankAccountSnapshot> {Data = new BankAccountSnapshot {Balance = 900}};
            store.SaveSnapshot(snapshot);

            database.Verify(db => db.InsertSnapshot(It.IsAny<SnapshotDetail>()), Times.Once());
        }
    }
}
