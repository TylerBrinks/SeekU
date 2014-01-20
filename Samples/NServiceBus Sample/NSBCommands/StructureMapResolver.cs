using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InMemorySample.EventHandlers;
using NSBDomain.CommandHandlers;
using NSBDomain.Commands;
using NSBDomain.Domain;
using NSBDomain.Events;
using SeekU;
using SeekU.Commanding;
using SeekU.Eventing;
using StructureMap;

namespace NSBCommands
{
    public class StructureMapResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public StructureMapResolver()
        {
            ObjectFactory.Initialize(x => x.Scan(scan =>
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

            _container = ObjectFactory.Container;
        }

        //[DebuggerStepThrough]
        public T Resolve<T>()
        {
            return _container.GetInstance<T>();
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return _container.GetAllInstances<T>();
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            var instances = _container.GetAllInstances(type);

            return instances.Cast<object>();
        }

        public object Resolve(Type type)
        {
            return _container.GetInstance(type);
        }

        public void Register<T, TK>()
            where T : class
            where TK : T
        {
            _container.Configure(x => x.For<T>().Use<TK>());
        }

        public void Register<T, TK>(Action<TK> configurationAction)
            where T : class
            where TK : T
        {
            _container.Configure(x => x.For<T>().Use<TK>().OnCreation(configurationAction));
        }

        public void Register<T>(T instance)
        {
            _container.Configure(x => x.For<T>().Use(instance));
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
