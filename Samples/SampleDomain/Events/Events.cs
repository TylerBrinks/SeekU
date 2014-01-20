using System;
using SeekU.Eventing;
using System.Runtime.Serialization;

namespace SampleDomain.Events
{
    [DataContract] // Only necessary for XML serialization with the text file example
    public class BankAccountEvent : DomainEvent
    {
        [DataMember] // Only necessary for XML serialization with the text file example
        public Guid Id { get; set; }
        [DataMember] // Only necessary for XML serialization with the text file example
        public double Amount { get; set; }
    }

    [DataContract] // Only necessary for XML serialization with the text file example
    public class AccountCreatedEvent : BankAccountEvent
    {
        public AccountCreatedEvent(Guid id, double amount)
        {
            Id = id;
            Amount = amount;
        }
    }

    [DataContract] // Only necessary for XML serialization with the text file example
    public class AccountDebitedEvent : BankAccountEvent
    {
        public AccountDebitedEvent(Guid id, double amount)
        {
            Id = id;
            Amount = amount;
        }
    }

    [DataContract] // Only necessary for XML serialization with the text file example
    public class AccountCreditedEvent : BankAccountEvent
    {
        public AccountCreditedEvent(Guid id, double amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}
