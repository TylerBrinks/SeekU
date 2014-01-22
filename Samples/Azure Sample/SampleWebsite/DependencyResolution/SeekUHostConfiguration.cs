using System;
using SampleDomain.Domain;
using SeekU.Commanding;
using SeekU.Eventing;
using SeekU.StructureMap;

namespace SampleWebsite.DependencyResolution
{
    public class SeekUResolver : SeekUStructureMapResolver
    {
        public SeekUResolver()
        {
            Container.Configure(x => x.Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssemblyContainingType<BankAccount>();
                scan.WithDefaultConventions();
                scan.ConnectImplementationsToTypesClosing(typeof(IHandleCommands<>));
                scan.ConnectImplementationsToTypesClosing(typeof(IHandleDomainEvents<>));
            }));
        }

        public override void Register<T, TK>()
        {
            Container.Configure(x => x
                .For<T>()
                .HttpContextScoped()
                .Use<TK>());
        }

        public override void Register<T, TK>(Action<TK> configurationAction)
        {
            Container.Configure(x => x
                .For<T>()
                .HttpContextScoped()
                .Use<TK>()
                .OnCreation(configurationAction));
        }

        public override void Register<T>(T instance)
        {
            Container.Configure(x => x
                .For<T>()
                .HttpContextScoped()
                .Use(instance));
        }
    }
}