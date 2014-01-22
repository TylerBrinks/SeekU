using System.Reflection;
using InMemorySample.EventHandlers;
using NSBDomain.CommandHandlers;
using NSBDomain.Commands;
using NSBDomain.Domain;
using NSBDomain.Events;
using SeekU.Commanding;
using SeekU.Eventing;
using SeekU.StructureMap;

namespace NSBCommands
{
    public class NsbStructureMapResolver : SeekUStructureMapResolver
    {
        public NsbStructureMapResolver()
        {
            Container.Configure(x => x.Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.Assembly(Assembly.GetExecutingAssembly());
                scan.AssemblyContainingType<BankAccount>();
                scan.WithDefaultConventions();

                x.For<IHandleCommands<CreateNewAccountCommand>>().Use<AccountHandler>();
                x.For<IHandleCommands<DebitAccountCommand>>().Use<AccountHandler>();
                x.For<IHandleCommands<CreditAccountCommand>>().Use<AccountHandler>();

                x.For<IHandleDomainEvents<AccountCreatedEvent>>().Use<BankAccountEventHandler>();
                x.For<IHandleDomainEvents<AccountCreditedEvent>>().Use<BankAccountEventHandler>();
                x.For<IHandleDomainEvents<AccountDebitedEvent>>().Use<BankAccountEventHandler>();
            }));
        }
    }
}
