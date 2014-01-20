using System;
using SeekU.Eventing;
using System.Runtime.Serialization;

namespace SampleDomain.Events
{
    [DataContract] // Necessary for json serialization
    public class BankAccountEvent : DomainEvent
    {
        [DataMember] // Necessary for json serialization
        public Guid Id { get; set; }
        [DataMember] // Necessary for json serialization
        public double Amount { get; set; }
    }

    [DataContract]
    public class AccountCreatedEvent : BankAccountEvent
    {
        public AccountCreatedEvent(Guid id, double amount)
        {
            Id = id;
            Amount = amount;
        }
    }

    [DataContract]
    public class AccountDebitedEvent : BankAccountEvent
    {
        public AccountDebitedEvent(Guid id, double amount)
        {
            Id = id;
            Amount = amount;
        }
    }

    [DataContract]
    public class AccountCreditedEvent : BankAccountEvent
    {
        public AccountCreditedEvent(Guid id, double amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}
