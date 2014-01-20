using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SeekU.Commanding;
using SeekU.Domain;
using SeekU.Eventing;
using SeekU.Tests.Fakes;

namespace SeekU.Tests.CommandingTests
{
    [TestFixture]
    public class CommandContextFixture
    {
        [Test]
        public void Context_Finds_Root_Objects()
        {
            var repo = new Mock<DomainRepository>();
            var resolver = new Mock<IDependencyResolver>();

            repo.Setup(r => r.GetById<TestDomain>(It.IsAny<Guid>())).Returns(new TestDomain());
            resolver.Setup(r => r.Resolve<DomainRepository>()).Returns(repo.Object);
            var context = new CommandContext(resolver.Object);

            var root = context.GetById<TestDomain>(SequentialGuid.NewId());

            Assert.IsNotNull(root);
            repo.VerifyAll();
        }

        [Test]
        public void Context_Does_Not_Finds_Missing_Root_Objects()
        {
            var repo = new Mock<DomainRepository>();
            var resolver = new Mock<IDependencyResolver>();

            repo.Setup(r => r.GetById<TestDomain>(It.IsAny<Guid>())).Returns((TestDomain)null);
            resolver.Setup(r => r.Resolve<DomainRepository>()).Returns(repo.Object);
            var context = new CommandContext(resolver.Object);

            var root = context.GetById<TestDomain>(SequentialGuid.NewId());

            Assert.IsNull(root);
            repo.VerifyAll();
        }

        [Test]
        public void Context_Publishes_Events()
        {
            var eventStore = new Mock<IEventStore>();
            var eventBus = new Mock<IEventBus>();
            var resolver = new Mock<IDependencyResolver>();

            eventStore.Setup(e => e.Insert(It.IsAny<Guid>(), It.IsAny<List<DomainEvent>>()));
            eventBus.Setup(e => e.PublishEvent(It.IsAny<DomainEvent>()));
            eventBus.Setup(e => e.PublishEvents(It.IsAny<List<DomainEvent>>()));

            resolver.Setup(r => r.Resolve<IEventStore>()).Returns(eventStore.Object);
            resolver.Setup(r => r.Resolve<IEventBus>()).Returns(eventBus.Object);

            var context = new CommandContext(resolver.Object);

            var root = new TestDomain(SequentialGuid.NewId());

            context.Finalize(root);

            eventBus.Verify(e => e.PublishEvents(It.IsAny<List<DomainEvent>>()), Times.Once);
        }

        [Test]
        public void Context_Loads_Root_From_Snapshot()
        {
            var eventStore = new Mock<IEventStore>();
            var snapshotStore = new Mock<ISnapshotStore>();
            var resolver = new Mock<IDependencyResolver>();

            eventStore.Setup(e => e.GetEvents(It.IsAny<Guid>(), It.IsAny<long>()));
            eventStore.Setup(e => e.Insert(It.IsAny<Guid>(), It.IsAny<List<DomainEvent>>()));
            snapshotStore.Setup(s => s.GetSnapshot<TestSnapshot>(It.IsAny<Guid>()))
                .Returns(new Snapshot<TestSnapshot>(Guid.NewGuid(), 3, new TestSnapshot { Value = 99 }));

            resolver.Setup(r => r.Resolve<IEventStore>()).Returns(eventStore.Object);
            resolver.Setup(r => r.Resolve<ISnapshotStore>()).Returns(snapshotStore.Object);
            resolver.Setup(r => r.Resolve<DomainRepository>()).Returns(new DomainRepository(snapshotStore.Object, eventStore.Object));

            var context = new CommandContext(resolver.Object);

            var root = context.GetById<TestDomainWithSnapshot>(SequentialGuid.NewId());

            Assert.AreEqual(3, root.Version);
        }

        [Test]
        public void Context_Replays_Post_Snapshot_Events()
        {
            var events = new List<DomainEvent>
            {
                new SomethingHappened{Sequence = 3},
                new SomethingHappened{Sequence = 4},
            };

            var eventStore = new Mock<IEventStore>();
            var snapshotStore = new Mock<ISnapshotStore>();
            var resolver = new Mock<IDependencyResolver>();

            eventStore.Setup(e => e.GetEvents(It.IsAny<Guid>(), It.IsAny<long>())).Returns(events);

            eventStore.Setup(e => e.Insert(It.IsAny<Guid>(), It.IsAny<List<DomainEvent>>()));
            snapshotStore.Setup(s => s.GetSnapshot<TestSnapshot>(It.IsAny<Guid>()))
                .Returns(new Snapshot<TestSnapshot>(Guid.NewGuid(), 2, new TestSnapshot { Value = 99 }));

            resolver.Setup(r => r.Resolve<IEventStore>()).Returns(eventStore.Object);
            resolver.Setup(r => r.Resolve<ISnapshotStore>()).Returns(snapshotStore.Object);
            resolver.Setup(r => r.Resolve<DomainRepository>())
                .Returns(new DomainRepository(snapshotStore.Object, eventStore.Object));

            var context = new CommandContext(resolver.Object);

            var root = context.GetById<TestDomainWithSnapshot>(SequentialGuid.NewId());

            Assert.AreEqual(4, root.Version);
            Assert.AreEqual(0, root.AppliedEvents.Count);
        }

        [Test]
        public void Context_Creates_Snapshots()
        {
            var eventStore = new Mock<IEventStore>();
            var eventBus = new Mock<IEventBus>();
            var snapshotStore = new Mock<ISnapshotStore>();
            var resolver = new Mock<IDependencyResolver>();

            eventStore.Setup(e => e.Insert(It.IsAny<Guid>(), It.IsAny<List<DomainEvent>>()));
            eventBus.Setup(e => e.PublishEvents(It.IsAny<List<DomainEvent>>()));
            snapshotStore.Setup(s => s.SaveSnapshot(It.IsAny<Snapshot<TestSnapshot>>()));

            resolver.Setup(r => r.Resolve<IEventStore>()).Returns(eventStore.Object);
            resolver.Setup(r => r.Resolve<ISnapshotStore>()).Returns(snapshotStore.Object);
            resolver.Setup(r => r.Resolve<IEventBus>()).Returns(eventBus.Object);

            var context = new CommandContext(resolver.Object);

            context.Finalize(new TestDomainWithSnapshot());

            eventStore.Verify(e => e.Insert(It.IsAny<Guid>(), It.IsAny<List<DomainEvent>>()), Times.Once);
            eventBus.Verify(e => e.PublishEvents(It.IsAny<List<DomainEvent>>()), Times.Once);
            snapshotStore.Verify(e => e.SaveSnapshot(It.IsAny<Snapshot<TestSnapshot>>()), Times.Once);
        }
    }
}
