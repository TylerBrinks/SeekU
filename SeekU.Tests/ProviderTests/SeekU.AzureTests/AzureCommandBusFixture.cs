using System;
using Moq;
using NUnit.Framework;
using SeekU.Azure.Commanding;
using SeekU.Commanding;

namespace SeekU.Tests.ProviderTests.SeekU.AzureTests
{
    [TestFixture]
    public class AzureCommandBusFixture
    {
        [Test]
        public void AzureCommandBus_Sends_Brokered_Messages()
        {
            var mockBus = new Mock<AzureCommandBus>{CallBase = true };
            mockBus.Setup(b => b.CreateQueue(It.IsAny<string>()));
            mockBus.Setup(b => b.SendMessage(It.IsAny<ICommand>(), It.IsAny<string>()));

            var bus = mockBus.Object;
            bus.AzureServiceBusConnectionString = "test";

            bus.Send(new Mock<ICommand>().Object);
            mockBus.VerifyAll();
        }

        [Test]
        public void AzureCommandBus_Requires_Connection_String()
        {
            var mockBus = new Mock<AzureCommandBus> { CallBase = true };

            var bus = mockBus.Object;

            Assert.Throws<ArgumentException>(() => bus.Send(new Mock<ICommand>().Object));
            mockBus.VerifyAll();
        }
    }
}
