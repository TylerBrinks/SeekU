using System;
using System.Collections.Generic;
using SeekU.Domain;

namespace SeekU.Tests.Fakes
{
    public class TestDomain : AggregateRoot
    {
        public bool OnSomethingHappenedCalled { get; set; }
        public bool ApplySomethingElseHappenedCalled { get; set; }
        public List<NewEventHappened> VersionedEvents = new List<NewEventHappened>();

        public TestDomain()
        {

        }

        public TestDomain(Guid id)
        {
            // Duplidated on purpose for testing event versioning
            ApplyEvent(new SomethingHappenedEvent());
            ApplyEvent(new SomethingElseHappened());
        }

        public void Modify()
        {
            ApplyEvent(new SomethingElseHappened());
        }

        protected void OnSomethingHappened(SomethingHappenedEvent evt)
        {
            OnSomethingHappenedCalled = true;
        }

        protected void Apply(SomethingElseHappened evt)
        {
            ApplySomethingElseHappenedCalled = true;
        }

        protected void Apply(NewEventHappened newEvent)
        {
            VersionedEvents.Add(newEvent);
        }
    }
}