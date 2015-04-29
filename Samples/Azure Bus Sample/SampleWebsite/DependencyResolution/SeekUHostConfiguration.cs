using System;
using SampleDomain.Domain;
using SeekU.Commanding;
using SeekU.Eventing;
using SeekU.StructureMap;
using StructureMap.Graph;
using StructureMap.Web.Pipeline;

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
                .LifecycleIs<HttpContextLifecycle>()
                .Use<TK>());
        }

        public override void Register<T, TK>(Action<TK> configurationAction)
        {
            Container.Configure(x => x
                .For<T>()
                .LifecycleIs<HttpContextLifecycle>()
                .Use<TK>()
                .OnCreation(typeof(TK).FullName, configurationAction));
        }

        public override void Register<T>(T instance)
        {
            Container.Configure(x => x
                .For<T>()
                .LifecycleIs<HttpContextLifecycle>()
                .Use(instance));
        }
    }
}