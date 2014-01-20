using System;
using NSBDomain.Events;

namespace NSBDomain.Domain
{
    public partial class BankAccount
    {
        private double _balance;

        public BankAccount()
        {
        }

        public BankAccount(Guid id, double startingBalance)
        {
            ApplyEvent(new AccountCreatedEvent(id, startingBalance));
        }

        public void CreditAccount(double amount)
        {
            ApplyEvent(new AccountCreditedEvent(Id, amount));
        }

        public void DebitAccount(double amount)
        {
            ApplyEvent(new AccountDebitedEvent(Id, amount));
        }
    }
}
