using System;
using NUnit.Framework;
using SeekU.Tests.Fakes;

namespace SeekU.Tests.DomainTests
{
    [TestFixture]
    public class AggregateRootFixture
    {
        [Test]
        public void AggregateRoot_Defaults_To_New_Id()
        {
            var root = new TestDomain();

            Assert.That(root.Id != Guid.Empty);
        }

        [Test]
        public void AggregateRoot_Increments_Version_For_Each_Applied_Event()
        {
            var root = new TestDomain(SequentialGuid.NewId());

            root.Modify();

            Assert.AreEqual(3, root.Version);
            Assert.AreEqual(3, root.AppliedEvents.Count);
        }
    }
}
