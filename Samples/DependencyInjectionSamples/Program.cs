using System;
using SampleDomain.Commands;
using SeekU;
using SeekU.Eventing;

namespace DependencyInjectionSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            UseNinject();
            UseStructureMap();
            UseWindsor();

            Console.Read();
        }

        static void UseStructureMap()
        {
            Console.WriteLine("Press a key to run with StructureMap");
            Console.ReadKey();
            // Configure using StructureMap
            var structureMapConfig = new HostConfiguration<StructureMapResolver>();
            structureMapConfig.ForSnapshotStore().Use<InMemorySnapshotStore>(store => ArbitraryConfigurationStep("StructureMap", store));
            structureMapConfig.For<IExample>().Use<Example>();

            Run(structureMapConfig);
        }

        static void UseNinject()
        {
            Console.WriteLine("\r\n================\r\n");
            Console.WriteLine("Press a key to run with Ninject");
            Console.ReadKey();

            // Configure using Ninject
            var ninjectConfig = new HostConfiguration<NinjectResolver>();
            ninjectConfig.ForSnapshotStore().Use<InMemorySnapshotStore>(store => ArbitraryConfigurationStep("Ninject", store));
            ninjectConfig.For<IExample>().Use<Example>();

            Run(ninjectConfig);
        }

        static void UseWindsor()
        {
            Console.WriteLine("\r\n================\r\n");
            Console.WriteLine("Press a key to run with Castle Windsor");
            Console.ReadKey();

            // Configure using Windsor
            var ninjectConfig = new HostConfiguration<WindsorResolver>();
            ninjectConfig.ForSnapshotStore().Use<InMemorySnapshotStore>(store => ArbitraryConfigurationStep("Windsor", store));
            ninjectConfig.For<IExample>().Use<Example>();

            Run(ninjectConfig);
        }

        static void ArbitraryConfigurationStep(string container, InMemorySnapshotStore store)
        {
            Console.WriteLine("This ran after creation {2} built {0}.  The hash code is {1}",
                store.GetType(), store.GetHashCode(), container);
        }

        static void Run(HostConfiguration config)
        {
            var host = new Host(config);

            var bus = host.GetCommandBus();

            var id = SequentialGuid.NewId();
            bus.Send(new CreateNewAccountCommand(id, 950));
            bus.Send(new DebitAccountCommand(id, 50));
            bus.Send(new CreditAccountCommand(id, 120));
            bus.Send(new DebitAccountCommand(id, 350));
        }
    }

    public interface IExample { }

    public class Example : IExample { }
}
