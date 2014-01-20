using System;
using SeekU.Eventing;

namespace NSBDomain.Events
{
    public class BankAccountEvent : DomainEvent
    {
        public Guid Id { get; set; }
        public double Amount { get; set; }
    }

    public class AccountCreatedEvent : BankAccountEvent
    {
        public AccountCreatedEvent(Guid id, double amount)
        {
            Id = id;
            Amount = amount;
        }
    }

    public class AccountDebitedEvent : BankAccountEvent
    {
        public AccountDebitedEvent(Guid id, double amount)
        {
            Id = id;
            Amount = amount;
        }
    }

    public class AccountCreditedEvent : BankAccountEvent
    {
        public AccountCreditedEvent(Guid id, double amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}
