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
                new SomethingHappened(),
                new SomethingHappened()
            };

            var handler1 = new Mock<IHandleDomainEvents<SomethingHappened>>();
            var handler2 = new Mock<IHandleDomainEvents<SomethingHappened>>();

            handler1.Setup(h => h.Handle(It.IsAny<SomethingHappened>()));
            handler2.Setup(h => h.Handle(It.IsAny<SomethingHappened>()));

            var resolver = new Mock<IDependencyResolver>();
            resolver.Setup(r => r.ResolveAll(It.IsAny<Type>())).Returns(
                new List<IHandleDomainEvents<SomethingHappened>> { handler1.Object, handler2.Object });

            var bus = new InProcessEventBus(resolver.Object);

            bus.PublishEvents(events);

            handler1.Verify(h => h.Handle(It.IsAny<SomethingHappened>()), Times.Exactly(2));
            handler2.Verify(h => h.Handle(It.IsAny<SomethingHappened>()), Times.Exactly(2));
        }
    }
}
