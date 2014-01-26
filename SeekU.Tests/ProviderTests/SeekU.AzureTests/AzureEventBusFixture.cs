using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SeekU.Azure.Commanding;
using SeekU.Eventing;

namespace SeekU.Tests.ProviderTests.SeekU.AzureTests
{
    [TestFixture]
    public class AzureEventBusFixture
    {
        [Test]
        public void AzureEventBus_Sends_Brokered_Messages()
        {
            var mockBus = new Mock<AzureEventBus>{CallBase = true };
            mockBus.Setup(b => b.CreateQueue(It.IsAny<string>()));
            mockBus.Setup(b => b.SendMessage(It.IsAny<object>(), It.IsAny<string>()));

            var bus = mockBus.Object;
            bus.AzureServiceBusConnectionString = "test";

            bus.PublishEvent(new Mock<DomainEvent>().Object);
            mockBus.VerifyAll();
        }

        [Test]
        public void AzureEventBus_Requires_Connection_String()
        {
            var mockBus = new Mock<AzureEventBus> { CallBase = true };

            var bus = mockBus.Object;

            Assert.Throws<ArgumentException>(() => bus.PublishEvent(new Mock<DomainEvent>().Object));
        }

        [Test]
        public void AzureEventBus_Publishes_Multiple_Events()
        {
            var mockBus = new Mock<AzureEventBus> { CallBase = true };
            mockBus.Setup(b => b.CreateQueue(It.IsAny<string>()));
            mockBus.Setup(b => b.SendMessage(It.IsAny<object>(), It.IsAny<string>()));

            var bus = mockBus.Object;
            bus.AzureServiceBusConnectionString = "test";

            bus.PublishEvents(new Mock<List<DomainEvent>>().Object);
            mockBus.VerifyAll();
        }
    }
}
