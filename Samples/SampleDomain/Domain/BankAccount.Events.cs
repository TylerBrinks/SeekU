using SampleDomain.Events;
using SeekU.Domain;

namespace SampleDomain.Domain
{
    public partial class BankAccount : AggregateRootWithSnapshot<BankAccountSnapshot>
    {
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

		// Don't remove this - it's called dynamically
        // It could also be "private void Apply(AccountCreatedEvent @event)"
        private void OnAccountCreated(AccountCreatedEvent @event)
        {
            Id = @event.Id;
            _balance = @event.Amount;
        }

        // Don't remove this - it's called dynamically
        // It could also be "private void OnAccountDebited(AccountDebitedEvent @event)"
        private void Apply(AccountDebitedEvent @event)
        {
            _balance -= @event.Amount;
        }

        // It could also be "private void OnAccountCredited(AccountDebitedEvent @event)"
        private void Apply(AccountCreditedEvent @event)
        {
            _balance += @event.Amount;
        }

        // It could also be "private void OnDebitCardAdded(AccountDebitedEvent @event)"
        private void Apply(DebitCardAddedEvent @event)
        {
            _cards.Add(new DebitCard(this, @event.CardId, @event.CardNumber));
        }
    }
}
