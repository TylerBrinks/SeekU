using SampleDomain.Events;
using SeekU.Domain;

namespace SampleDomain.Domain
{
    public partial class BankAccount : AggregateRootWithSnapshot<BankAccountSnapshot>
    {
		// Don't remove this - it's called dynamically
        private void Apply(AccountCreatedEvent @event)
        {
            Id = @event.Id;
            _balance = @event.Amount;
        }

        // Don't remove this - it's called dynamically
        private void Apply(AccountDebitedEvent @event)
        {
            _balance -= @event.Amount;
        }

        private void Apply(AccountCreditedEvent @event)
        {
            _balance += @event.Amount;
        }

        public override void LoadFromSnapshot(BankAccountSnapshot snapshot)
        {
            _balance = snapshot.Balance;
        }

        protected override BankAccountSnapshot CreateSnapshot()
        {
            return new BankAccountSnapshot { Balance = _balance };
        }

        protected override bool ShouldCreateSnapshot()
        {
            // Create a snapshot every 3 events
            return Version % 3 == 0;
        }
    }
}
