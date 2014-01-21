using System;
using System.Runtime.Serialization;
using SeekU.Commanding;

namespace SampleDomain.Commands
{
    [DataContract]
    public class CreateNewAccountCommand : ICommand
    {
        public CreateNewAccountCommand(Guid id, double startingBalance)
        {
            Id = id;
            StartingBalance = startingBalance;
        }
        
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public double StartingBalance { get; set; }
    }
    
    [DataContract]
    public class DebitAccountCommand : ICommand
    {
        public DebitAccountCommand(Guid aggregateId, double amountToDeduct)
        {
            Id = aggregateId;
            Amount = amountToDeduct;
        }

        [DataMember]
        public Guid Id { get; private set; }
        [DataMember]
        public double Amount { get; private set; }
    }

    [DataContract]
    public class CreditAccountCommand : ICommand
    {
        public CreditAccountCommand(Guid aggregateId, double amountToDeduct)
        {
            Id = aggregateId;
            Amount = amountToDeduct;
        }

        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public double Amount { get; private set; }
    }
}
