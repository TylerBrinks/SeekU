using System;
using SeekU.Commanding;

namespace NSBDomain.Commands
{
    public class CreateNewAccountCommand : ICommand, NServiceBus.ICommand
    {
        public Guid Id { get; set; }
        public double StartingBalance { get; set; }

        public CreateNewAccountCommand(Guid id, double startingBalance)
        {
            Id = id;
            StartingBalance = startingBalance;
        }
    }

    public class DebitAccountCommand : ICommand, NServiceBus.ICommand
    {
        public Guid Id { get; private set; }
        public double Amount { get; private set; }

        public DebitAccountCommand(Guid aggregateId, double amountToDeduct)
        {
            Id = aggregateId;
            Amount = amountToDeduct;
        }
    }

    public class CreditAccountCommand : ICommand, NServiceBus.ICommand
    {
        public Guid Id { get; set; }
        public double Amount { get; private set; }

        public CreditAccountCommand(Guid aggregateId, double amountToDeduct)
        {
            Id = aggregateId;
            Amount = amountToDeduct;
        }
    }
}
