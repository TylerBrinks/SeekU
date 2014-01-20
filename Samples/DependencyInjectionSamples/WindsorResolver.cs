using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DependencyInjectionSamples.EventHandlers;
using SampleDomain.CommandHandlers;
using SampleDomain.Commands;
using SampleDomain.Events;
using SeekU;
using SeekU.Commanding;
using SeekU.Eventing;

namespace DependencyInjectionSamples
{
    public class WindsorResolver : IDependencyResolver
    {
        private readonly WindsorContainer _container;

        public WindsorResolver()
        {
            _container = new WindsorContainer();
            _container.Register(Component.For<IHandleCommands<CreateNewAccountCommand>>()
                .ImplementedBy<AccountHandler>().Named("CreateNewAccountCommand"));
            _container.Register(Component.For<IHandleCommands<DebitAccountCommand>>()
                .ImplementedBy<AccountHandler>().Named("DebitAccountCommand"));
            _container.Register(Component.For<IHandleCommands<CreditAccountCommand>>()
                .ImplementedBy<AccountHandler>().Named("CreditAccountCommand"));

            _container.Register(Component.For<IHandleDomainEvents<AccountCreatedEvent>>()
                .ImplementedBy<BankAccountEventHandler>().Named("AccountCreatedEvent"));
            _container.Register(Component.For<IHandleDomainEvents<AccountDebitedEvent>>()
                .ImplementedBy<BankAccountEventHandler>().Named("AccountDebitedEvent"));
            _container.Register(Component.For<IHandleDomainEvents<AccountCreditedEvent>>()
                .ImplementedBy<BankAccountEventHandler>().Named("AccountCreditedEvent"));
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return _container.ResolveAll<T>();
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            return _container.ResolveAll(type).Cast<object>();
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public void Register<T, TK>() where T : class where TK : T
        {
            _container.Register(Component.For<T>().ImplementedBy<TK>().LifestyleTransient());
        }

        public void Register<T, TK>(Action<TK> configurationAction) where T : class where TK : T
        {
            _container.Register(Component.For<T>().ImplementedBy<TK>()
                .IsDefault()
                .LifestyleTransient()
                .Named(Guid.NewGuid().ToString())
                .OnCreate((kernel, instance) => configurationAction((TK)instance)));
        }

        public void Register<T>(T instance)
        {
            _container.Register(Component.For(typeof (T)).Instance(instance).LifestyleTransient());
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
