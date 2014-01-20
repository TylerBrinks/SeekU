using System;
using SeekU.Domain;

namespace SeekU.Tests.Fakes
{
    public class TestDomain : AggregateRoot
    {
        public TestDomain()
        {

        }

        public TestDomain(Guid id)
        {
            // Duplidated on purpose for testing event versioning
            ApplyEvent(new SomethingHappened());
            ApplyEvent(new SomethingHappened());
        }

        public void Modify()
        {
            ApplyEvent(new SomethingElseHappened());
        }

        protected void Apply(SomethingHappened evt)
        {

        }

        protected void Apply(SomethingElseHappened evt)
        {
            
        }
    }
}