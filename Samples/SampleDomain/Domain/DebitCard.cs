using System;
using System.Collections.Generic;
using System.Linq;
using SampleDomain.Events;
using SeekU.Domain;

namespace SampleDomain.Domain
{
    public class DebitCard : Entity
    {
        private readonly List<Transaction> _transactions = new List<Transaction>();

        public DebitCard(AggregateRoot parent, Guid cardId, string cardNumber) : base(parent, cardId)
        {
            CardNumber = cardNumber;
        }

        public void DebitAccount(string merchant, double amount)
        {
            ApplyEvent(new DebitCardChargedEvent(Id, merchant, amount));
        }

        public void Apply(DebitCardChargedEvent chargedEvent)
        {
            _transactions.Add(new Transaction
            {
                Amount = chargedEvent.Amount, 
                Merchant = chargedEvent.Merchant
            });
        }

        public double GetTotalTransactions()
        {
            return _transactions.Sum(t => t.Amount);
        }

        public Guid AccountId { get; set; }

        public string CardNumber { get; set; }
    }
}
