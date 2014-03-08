using SeekU.Eventing;

namespace SeekU.Tests.Fakes
{
    public class SomethingHappenedEvent : DomainEvent
    {
        
    }

    public class SomethingElseHappened : DomainEvent
    {
        
    }

    public class OldEventHappened : DomainEvent
    {
        public string FirstName { get; set; }

        public override DomainEvent UpgradeVersion()
        {
            return new IntermediateEventHappened
            {
                FirstName = FirstName,
                LastName = "Unknown"
            };
        }
    }

    public class IntermediateEventHappened : DomainEvent
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override DomainEvent UpgradeVersion()
        {
            return new NewEventHappened
            {
                FirstName = FirstName,
                LastName = LastName,
                Age = -1
            };
        }
    }

    public class NewEventHappened : DomainEvent
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }


    }
}