using System;
using NUnit.Framework;
using SampleDomain.Domain;

namespace SeekU.Tests.DomainTests
{
    [TestFixture]
    public class EntityFixture
    {
        [Test]
        public void Entity_Events_Are_Managed_By_The_Aggregate_Root()
        {
            var account = SetupAccount();

            Assert.AreEqual(200, account.GetTotalCardTransactions());
            Assert.AreEqual(6, account.Version);
            Assert.AreEqual(6, account.AppliedEvents.Count);
        }

        [Test]
        public void Entities_Are_Build_From_History_Replay()
        {
            var events  = SetupAccount().AppliedEvents;

            var account = new BankAccount();

            account.ReplayEvents(events);

            Assert.AreEqual(200, account.GetTotalCardTransactions());
            Assert.AreEqual(6, account.Version);
            Assert.AreEqual(0, account.AppliedEvents.Count);
        }

        private static BankAccount SetupAccount()
        {
            var accountId = Guid.NewGuid();
            var card1Id = Guid.NewGuid();
            var card2Id = Guid.NewGuid();

            var account = new BankAccount(accountId, 1000);
            account.AddDebitCard(card1Id, "123-456");
            account.AddDebitCard(card2Id, "987-654");

            account.SwipeDebitCard(card1Id, "Amazon", 35);
            account.SwipeDebitCard(card2Id, "Grocery store", 120);
            account.SwipeDebitCard(card1Id, "Gas station", 45);

            return account;
        }
    }
}
