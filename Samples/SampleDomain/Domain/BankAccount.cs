using System;
using System.Collections.Generic;
using System.Linq;
using SampleDomain.Events;

namespace SampleDomain.Domain
{
    public partial class BankAccount
    {
        private double _balance;
        private readonly List<DebitCard> _cards = new List<DebitCard>();
 
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

        public void AddDebitCard(Guid cardId, string cardNumber)
        {
            ApplyEvent(new DebitCardAddedEvent(Id, cardId, cardNumber));
        }

        public void SwipeDebitCard(Guid cardId, string merchant, double amount)
        {
            _cards.First(c => c.Id == cardId).DebitAccount(merchant, amount);
        }

        public double GetTotalCardTransactions()
        {
            return _cards.Sum(c => c.GetTotalTransactions());
        }
    }
}
