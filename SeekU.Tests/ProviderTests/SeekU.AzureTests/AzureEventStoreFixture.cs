using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SampleDomain.Events;
using SeekU.Azure.Eventing;
using SeekU.Eventing;
using SeekU.Azure;
using SeekU.Tests.Properties;

namespace SeekU.Tests.ProviderTests.SeekU.AzureTests
{
    [TestFixture]
    public class AzureEventStoreFixture
    {
        [Test]
        public void AzureEventStore_Deserializes_Events()
        {
            var id = Guid.NewGuid();
            var events = new List<EventStream>
            {
                new EventStream(id, 2)
                {
                    EventData = Resources.JsonSerializedDebitEvent,
                    SequenceEnd = 2
                },
                new EventStream(id, 1)
                {
                    EventData = Resources.JsonSerializedNewAccountEvent,
                    SequenceEnd = 1
                }
            };

            var database = new Mock<IAzureStorageRepository>();
            database.Setup(db => db.GetEventStream(It.IsAny<Guid>(), It.IsAny<long>())).Returns(events);

            var store = new AzureTableEventStore
            {
                GetRepository = () => database.Object
            };

            var eventStream = store.GetEvents(id, 1).ToList();

            Assert.AreEqual(2, eventStream.Count());
            Assert.AreEqual(1, eventStream[0].Sequence);
            Assert.AreEqual(2, eventStream[1].Sequence);
            Assert.IsInstanceOf<AccountCreatedEvent>(eventStream[0]);
            Assert.IsInstanceOf<AccountDebitedEvent>(eventStream[1]);
        }

        [Test]
        public void AzureEventStore_Serialized_Events()
        {
            var database = new Mock<IAzureStorageRepository>();
            database.Setup(db => db.InsertEvents(It.IsAny<EventStream>()));

            var store = new AzureTableEventStore
            {
                GetRepository = () => database.Object
            };

            var events = new List<DomainEvent>
            {
                new AccountCreatedEvent(Guid.NewGuid(), 100),
                new AccountDebitedEvent(Guid.NewGuid(), 50)
            };

            store.Insert(Guid.NewGuid(), events);

            database.Verify(db => db.InsertEvents(It.IsAny<EventStream>()), Times.Once);
        }
    }
}
