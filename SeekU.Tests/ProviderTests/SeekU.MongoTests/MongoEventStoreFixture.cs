using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using SampleDomain.Events;
using SeekU.Eventing;
using SeekU.MongoDB;
using SeekU.MongoDB.Eventing;

namespace SeekU.Tests.ProviderTests.SeekU.MongoTests
{
    [TestFixture]
    public class MongoEventStoreFixture
    {
        [Test]
        public void MongoEventStore_Deserializes_Events()
        {
            var id = Guid.NewGuid();
            var events = new List<EventStream>
            {
                new EventStream
                {
                    AggregateRootId = id,
                    DateCreated = DateTime.Now,
                    EventData = new List<DomainEvent>{ new AccountDebitedEvent(Guid.NewGuid(), 100){ Sequence = 2} },
                    Id = ObjectId.GenerateNewId(),
                    SequenceStart = 2,
                    SequenceEnd = 2
                },
                new EventStream
                {
                    AggregateRootId = id,
                    DateCreated = DateTime.Now,
                    EventData = new List<DomainEvent>{ new AccountCreatedEvent(Guid.NewGuid(), 100){ Sequence = 1} },
                    Id = ObjectId.GenerateNewId(),
                    SequenceStart = 1,
                    SequenceEnd = 1
                }
            };

            var database = new Mock<IMongoRepository>();
            database.Setup(db => db.GetEventStream(It.IsAny<Guid>(), It.IsAny<long>())).Returns(events);

            var store = new MongoEventStore
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
        public void MongoEventStore_Serialized_Events()
        {
            var database = new Mock<IMongoRepository>();
            database.Setup(db => db.InsertEvents(It.IsAny<EventStream>()));

            var store = new MongoEventStore
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
