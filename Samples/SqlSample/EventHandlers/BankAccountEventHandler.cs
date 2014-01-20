using System;
using System.Configuration;
using SampleDomain.Events;
using SeekU.Eventing;

namespace SqlSample.EventHandlers
{
    public class BankAccountEventHandler :
       IHandleDomainEvents<AccountCreatedEvent>,
       IHandleDomainEvents<AccountDebitedEvent>,
       IHandleDomainEvents<AccountCreditedEvent>
    {
        //private static readonly OrmLiteConnectionFactory ConnectionFactory;

        static BankAccountEventHandler()
        {
            //try
            //{
            //    // Create the 
            //    ConnectionFactory = new OrmLiteConnectionFactory(
            //        ConfigurationManager.ConnectionStrings["SeekUEventStore"].ConnectionString,
            //        SqlServerDialect.Provider);

            //    using (var db = ConnectionFactory.OpenDbConnection())
            //    {
            //        db.CreateTableIfNotExists<BankAccountReadModel>();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Unable to connect to SQL Server and create EventStream table.  Please check your 'SeekUEventStore' connection string and connection permissions.", ex);
            //}
        }

        public void Handle(AccountCreatedEvent domainEvent)
        {
            // Update the Read database
            Console.WriteLine("Inserting a new account record with a starting balance of {0}", domainEvent.Amount);

            //using (var db = ConnectionFactory.OpenDbConnection())
            //{
            //    var account = new BankAccountReadModel
            //    {
            //        Id = domainEvent.Id,
            //        CurrentBalance = domainEvent.Amount
            //    };
                
            //    db.Insert(account);
            //}
        }

        public void Handle(AccountDebitedEvent domainEvent)
        {
            // Update the Read database
            Console.WriteLine("Updating account record -{0}", domainEvent.Amount.ToString("C"));

            //using (var db = ConnectionFactory.OpenDbConnection())
            //{
            //    var account = db.Single<BankAccountReadModel>(new { domainEvent.Id});
            //    account.CurrentBalance -= domainEvent.Amount;

            //    db.Update(account);
            //}
        }

        public void Handle(AccountCreditedEvent domainEvent)
        {
            // Update the Read database
            Console.WriteLine("Updating account record {0}", domainEvent.Amount.ToString("C"));

            //using (var db = ConnectionFactory.OpenDbConnection())
            //{
            //    var account = db.Single<BankAccountReadModel>(new { domainEvent.Id });
            //    account.CurrentBalance += domainEvent.Amount;

            //    db.Update(account);
            //}
        }
    }
}
