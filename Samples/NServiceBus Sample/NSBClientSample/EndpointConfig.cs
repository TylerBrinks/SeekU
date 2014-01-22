using System;
using NSBCommands;
using NSBDomain.Commands;
using NServiceBus;
using SeekU;

namespace NSBClientSample
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Client 
    {
    }

    public class CommandSender : IWantToRunWhenBusStartsAndStops
    {
        // Injected by NServiceBus
        public IBus Bus { get; set; }

        public void Start()
        {
            Console.WriteLine("Press enter to send bank account commands");
            Console.Read();

            // There are all kinds of options for sending objects across application
            // domain boundaries.  Calling SendSeekU is demonstrating that SeekU 
            // has full support for swapping out the major functional areas.
            // This first example uses a custom ICommandBus implementation that
            // wraps the NServiceBus "IBus" interface.

            // Send a command using SeekU with NServiceBus as the transport.
            SendSeekU();

            // This alternative example sends commands directly through NServiceBus 
            // without using SeekU except for the command object's interface definition.
            // SeekU can still be used on the receiving end (NSBServerSample) to handle.
            // The rest of the CQRS Aggregate Root lifecycle.  
            
            //SendNSB();

            // Please keep in mind that you don't have to send just commands.  You could 
            // just as easily send events to the server instead of commands.  That would
            // allow the client to update your write model and the server to update 
            // the read model.  In that case you'd use a custom IEventBus implementation
            // instead of a custom ICommandBus implementation.  It's totally up to the
            // needs of your application.

            Console.WriteLine("All messages sent");
            Console.Read();
        }

        private void SendSeekU()
        {
            // Use a custom configuration with our own ICommandBus implementation
            var host = new SeekUHostConfiguration<NsbStructureMapResolver>();
            // Configure commands to be published via NServiceBus instead of the 
            // in-memory bus.
            host.ForCommandBus().Use<NServiceCommandBus>();
            // The NServiceCommandBus requires IBus as a constructor parameter.
            // This tells the resolver what to use for that dependency.
            host.For<IBus>().Use(Bus);

            var seekUBus = host.GetCommandBus();

            var id = Guid.NewGuid();

            Console.WriteLine("Sending create account command");
            seekUBus.Send(new CreateNewAccountCommand(id, 950));
            Console.WriteLine("Sending debit account command");
            seekUBus.Send(new DebitAccountCommand(id, 50));
            Console.WriteLine("Sending credit account command");
            seekUBus.Send(new CreditAccountCommand(id, 120));
            Console.WriteLine("Sending debit account command");
            seekUBus.Send(new DebitAccountCommand(id, 350));
        }

        private void SendNSB()
        {
            var id = Guid.NewGuid();
            Console.WriteLine("Sending create account command");
            Bus.Send(new CreateNewAccountCommand(id, 950));
            Console.WriteLine("Sending debit account command");
            Bus.Send(new DebitAccountCommand(id, 50));
            Console.WriteLine("Sending credit account command");
            Bus.Send(new CreditAccountCommand(id, 120));
            Console.WriteLine("Sending debit account command");
            Bus.Send(new DebitAccountCommand(id, 350));
        }

        public void Stop()
        {
        }
    }
}
