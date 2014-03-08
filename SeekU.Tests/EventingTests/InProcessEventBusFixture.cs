using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SeekU.Eventing;
using SeekU.Tests.Fakes;

namespace SeekU.Tests.EventingTests
{
    [TestFixture]
    public class InProcessEventBusFixture
    {
        [Test]
        public void InProcessEventBus_Publishes_Events()
        {
            var events = new List<DomainEvent>
            {
                new SomethingHappenedEvent(),
                new SomethingHappenedEvent()
            };

            var handler1 = new Mock<IHandleDomainEvents<SomethingHappenedEvent>>();
            var handler2 = new Mock<IHandleDomainEvents<SomethingHappenedEvent>>();

            handler1.Setup(h => h.Handle(It.IsAny<SomethingHappenedEvent>()));
            handler2.Setup(h => h.Handle(It.IsAny<SomethingHappenedEvent>()));

            var resolver = new Mock<IDependencyResolver>();
            resolver.Setup(r => r.ResolveAll(It.IsAny<Type>())).Returns(
                new List<IHandleDomainEvents<SomethingHappenedEvent>> { handler1.Object, handler2.Object });

            var bus = new InProcessEventBus(resolver.Object);

            bus.PublishEvents(events);

            handler1.Verify(h => h.Handle(It.IsAny<SomethingHappenedEvent>()), Times.Exactly(2));
            handler2.Verify(h => h.Handle(It.IsAny<SomethingHappenedEvent>()), Times.Exactly(2));
        }
    }
}
