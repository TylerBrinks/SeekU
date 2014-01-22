using System;
using NSBCommands;
using NSBDomain.Commands;
using SeekU;
using SeekU.Commanding;

namespace NSBServerSample
{
    using NServiceBus;

	/*
		This class configures this endpoint as a Server. More information about how to configure the NServiceBus host
		can be found here: http://particular.net/articles/the-nservicebus-host
	*/
	public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
    {
	    public void Init()
	    {
	        Configure.Transactions.Disable();
	        Configure.With()
                .DefaultBuilder()
                .UseInMemoryTimeoutPersister()
                .InMemoryFaultManagement()
	            .InMemorySagaPersister()
	            .InMemorySubscriptionStorage();
	    }
    }

    public class BankAccountHandler : 
        IHandleMessages<CreateNewAccountCommand>,
        IHandleMessages<DebitAccountCommand>,
        IHandleMessages<CreditAccountCommand>
    {

        private static readonly SeekUHostConfiguration<NsbStructureMapResolver> Host;

        static BankAccountHandler()
        {
            // Use the default in memory providers, so no configuration is necessary.
            Host = new SeekUHostConfiguration<NsbStructureMapResolver>();

            //Host = new Host(config);
        }
        public void Handle(CreateNewAccountCommand message)
        {
            Console.WriteLine("Create account message received");

            Host.GetCommandBus().Send(message);
        }

        public void Handle(DebitAccountCommand message)
        {
            Console.WriteLine("Debiting account message received");

            Host.GetCommandBus().Send(message);
        }

        public void Handle(CreditAccountCommand message)
        {
            Console.WriteLine("Debiting account message received");

            Host.GetCommandBus().Send(message);
        }
    }
}
