using System.Diagnostics;
using SampleDomain.Events;
using SeekU.Eventing;

namespace WorkerRoleQueueSample.EventHandlers
{
    public class BankAccountEventHandler :
       IHandleDomainEvents<AccountCreatedEvent>,
       IHandleDomainEvents<AccountDebitedEvent>,
       IHandleDomainEvents<AccountCreditedEvent>
    {
        public void Handle(AccountCreatedEvent domainEvent)
        {
            // Here is where you'd update your Read database
            Trace.WriteLine("##############################################");
            Trace.WriteLine("Account was created via Azure " + domainEvent.Amount.ToString("C"));
            Trace.WriteLine("##############################################");
        }

        public void Handle(AccountDebitedEvent domainEvent)
        {
            // Here is where you'd update your Read database
            Trace.WriteLine("##############################################");
            Trace.WriteLine("Account was debited via azure " + domainEvent.Amount.ToString("C"));
            Trace.WriteLine("##############################################");
        }

        public void Handle(AccountCreditedEvent domainEvent)
        {
            // Here is where you'd update your Read database
            Trace.WriteLine("##############################################");
            Trace.WriteLine("Account was credited ia Azure " + domainEvent.Amount.ToString("C"));
            Trace.WriteLine("##############################################");
        }
    }
}
