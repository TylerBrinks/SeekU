using System;
using SeekU.Eventing;
using System.Runtime.Serialization;

namespace SampleDomain.Events
{
    // It's likely you don't need all of the data contract and known attributes
    // in your own event bus.  These are here for the json file serializer
    // and the Azure BrokeredMessage serializer/deserializer.

    [DataContract] // Necessary for Azure and json serialization
    [KnownType(typeof(AccountCreatedEvent))]  // For Azure event deserialization
    [KnownType(typeof(AccountDebitedEvent))]  // For Azure event deserialization
    [KnownType(typeof(AccountCreditedEvent))] // For Azure event deserialization
    public class BankAccountEvent : DomainEvent
    {
        [DataMember] // Necessary Azure and for json serialization
        public Guid Id { get; set; }

        [DataMember] // Necessary Azure and for json serialization
        public double Amount { get; set; }
    }

    [DataContract]
    public class AccountCreatedEvent : BankAccountEvent
    {
        public AccountCreatedEvent()
        {
            
        }
        public AccountCreatedEvent(Guid id, double amount)
        {
            Id = id;
            Amount = amount;
        }
    }

    [DataContract]
    public class AccountDebitedEvent : BankAccountEvent
    {
        public AccountDebitedEvent()
        {
            
        }
        public AccountDebitedEvent(Guid id, double amount)
        {
            Id = id;
            Amount = amount;
        }
    }

    [DataContract]
    public class AccountCreditedEvent : BankAccountEvent
    {
        public AccountCreditedEvent()
        {
            
        }
        public AccountCreditedEvent(Guid id, double amount)
        {
            Id = id;
            Amount = amount;
        }
    }

    [DataContract]
    public class DebitCardAddedEvent : DomainEvent
    {
        public DebitCardAddedEvent()
        {
            
        }
        public DebitCardAddedEvent(Guid accountId, Guid cardId, string cardNumber)
        {
            AccountId = accountId;
            CardId = cardId;
            CardNumber = cardNumber;
        }

        [DataMember]
        public Guid AccountId { get; set; }
        
        [DataMember]
        public Guid CardId { get; set; }
        
        [DataMember]
        public string CardNumber { get; set; }
    }
    
    [DataContract]
    public class DebitCardSwipedEvent : BankAccountEvent
    {
        public DebitCardSwipedEvent(Guid cardId, double amount)
        {
            Id = cardId;
            Amount = amount;
        }
    }

    [DataContract]
    public class DebitCardChargedEvent : DomainEntityEvent
    {
        public DebitCardChargedEvent(Guid entityId, string merchant, double amount)
        {
            EntityId = entityId;
            Merchant = merchant;
            Amount = amount;
        }

        [DataMember]
        public double Amount { get; set; }
        
        [DataMember]
        public string Merchant { get; set; }
    }
}
