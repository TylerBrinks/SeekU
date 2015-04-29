using System;
using SampleDomain.Commands;
using SampleDomain.Domain;
using SeekU;
using SeekU.Azure.Eventing;
using SeekU.Commanding;
using SeekU.Eventing;
using SeekU.StructureMap;
using StructureMap;
using StructureMap.Graph;

namespace AzureStorageSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Use MongoDB for event and snapshot storage
            var host = new SeekUHostConfiguration<SeekUDemoDependencyResolver>();
            // Update with your connection string
            const string connectionString =
                "DefaultEndpointsProtocol=https;AccountName=[Your account name];AccountKey[Your account key]";

            host
                .ForEventStore().Use<AzureTableEventStore>(store =>
                {
                    store.TableConnectionString = connectionString;                    
                })
                .ForSnapshotStore().Use<AzureBlobSnapshotStore>(store =>
                {
                    store.BlobConnectionString = connectionString;
                });

            var bus = host.GetCommandBus();

            // I'm not a proponent of Guids for primary keys.  This method returns
            // a sequential Guid to make database sorting behave like integers.
            // http://www.informit.com/articles/article.asp?p=25862
            var id = SequentialGuid.NewId();

            // Create the account
            bus.Send(new CreateNewAccountCommand(id, 95));
            Console.WriteLine("Azure event created");

            // Use the account to create a history of events including a snapshot
            bus.Send(new DebitAccountCommand(id, 5));
            Console.WriteLine("Azure event created");

            bus.Send(new CreditAccountCommand(id, 12));
            Console.WriteLine("Azure event created");
            Console.WriteLine("Azure snapshot created");

            bus.Send(new DebitAccountCommand(id, 35));
            Console.WriteLine("Azure event created");

            Console.Read();
        }
    }

    public class SeekUDemoDependencyResolver : SeekUStructureMapResolver
    {
        public SeekUDemoDependencyResolver()
        {
            Container.Configure(x => x.Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssemblyContainingType<BankAccount>();
                scan.WithDefaultConventions();
                scan.ConnectImplementationsToTypesClosing(typeof(IHandleCommands<>));
                scan.ConnectImplementationsToTypesClosing(typeof(IHandleDomainEvents<>));
            }));

            Container = ObjectFactory.Container;
        }
    }
}
