using SeekU.Domain;

namespace SeekU.Tests.Fakes
{
    public class TestDomainWithSnapshot : AggregateRootWithSnapshot<TestSnapshot>
    {
        public int Value { get; set; }

        protected void Apply(SomethingHappened evt)
        {

        }

        protected override bool ShouldCreateSnapshot()
        {
            return true;
        }

        protected override TestSnapshot CreateSnapshot()
        {
            return new TestSnapshot {Value = Value};
        }

        public override void LoadFromSnapshot(TestSnapshot snapshot)
        {
            Value = snapshot.Value;
        }
    }
}