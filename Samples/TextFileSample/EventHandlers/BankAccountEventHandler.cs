using System;
using SampleDomain.Events;
using SeekU.Eventing;

namespace TextFileSample.EventHandlers
{
    public class BankAccountEventHandler :
        IHandleDomainEvents<AccountCreatedEvent>,
        IHandleDomainEvents<AccountDebitedEvent>,
        IHandleDomainEvents<AccountCreditedEvent>
    {
        public void Handle(AccountCreatedEvent domainEvent)
        {
            // Here is where you'd update your Read database
            Console.WriteLine("Account was created with a starting balance of {0}", domainEvent.Amount);
        }

        public void Handle(AccountDebitedEvent domainEvent)
        {
            // Here is where you'd update your Read database
            Console.WriteLine("Account was debited -{0}", domainEvent.Amount.ToString("C"));
        }

        public void Handle(AccountCreditedEvent domainEvent)
        {
            // Here is where you'd update your Read database
            Console.WriteLine("Account was credited {0} ", domainEvent.Amount.ToString("C"));
        }
    }
}
