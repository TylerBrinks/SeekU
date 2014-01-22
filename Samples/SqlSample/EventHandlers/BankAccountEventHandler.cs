using System;
using SampleDomain.Events;
using SeekU.Eventing;

namespace SqlSample.EventHandlers
{
    public class BankAccountEventHandler :
       IHandleDomainEvents<AccountCreatedEvent>,
       IHandleDomainEvents<AccountDebitedEvent>,
       IHandleDomainEvents<AccountCreditedEvent>
    {
        static BankAccountEventHandler()
        {
            using (var db = new PetaPoco.Database("DemoConnectionString"))
            {
                var count = db.ExecuteScalar<int>(@"select count(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'BankAccountReadModel'");

                if (count > 0)
                {
                    return;
                }

                db.Execute("CREATE TABLE BankAccountReadModel ([Id] [uniqueidentifier] NOT NULL,[CurrentBalance] [float] NOT NULL,PRIMARY KEY CLUSTERED ([Id] ASC))");
            }
        }

        public void Handle(AccountCreatedEvent domainEvent)
        {
            // Update the Read database
            Console.WriteLine("Inserting a new account record with a starting balance of {0}", domainEvent.Amount);

            using (var db = new PetaPoco.Database("DemoConnectionString"))
            {
                var account = new BankAccountReadModel
                {
                    Id = domainEvent.Id,
                    CurrentBalance = domainEvent.Amount
                };

                db.Insert(account);
            }
        }

        public void Handle(AccountDebitedEvent domainEvent)
        {
            // Update the Read database
            Console.WriteLine("Updating account record -{0}", domainEvent.Amount.ToString("C"));

            using (var db = new PetaPoco.Database("DemoConnectionString"))
            {
                var account = db.Single<BankAccountReadModel>(domainEvent.Id);
                account.CurrentBalance -= domainEvent.Amount;

                db.Update(account);
            }
        }

        public void Handle(AccountCreditedEvent domainEvent)
        {
            // Update the Read database
            Console.WriteLine("Updating account record {0}", domainEvent.Amount.ToString("C"));

            using (var db = new PetaPoco.Database("DemoConnectionString"))
            {
                var account = db.Single<BankAccountReadModel>(domainEvent.Id);
                account.CurrentBalance += domainEvent.Amount;

                db.Update(account);
            }
        }
    }
}
