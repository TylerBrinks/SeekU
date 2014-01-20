using System;
using System.Collections.Generic;
using DependencyInjectionSamples.EventHandlers;
using Ninject;
using SampleDomain.CommandHandlers;
using SampleDomain.Commands;
using SampleDomain.Events;
using SeekU;
using SeekU.Commanding;
using SeekU.Eventing;

namespace DependencyInjectionSamples
{
    public class NinjectResolver : IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectResolver()
        {
            _kernel = new StandardKernel();
            _kernel.Bind<IHandleCommands<CreateNewAccountCommand>>().To<AccountHandler>();
            _kernel.Bind<IHandleCommands<DebitAccountCommand>>().To<AccountHandler>();
            _kernel.Bind<IHandleCommands<CreditAccountCommand>>().To<AccountHandler>();

            _kernel.Bind<IHandleDomainEvents<AccountCreditedEvent>>().To<BankAccountEventHandler>();
            _kernel.Bind<IHandleDomainEvents<AccountDebitedEvent>>().To<BankAccountEventHandler>();
            _kernel.Bind<IHandleDomainEvents<AccountCreditedEvent>>().To<BankAccountEventHandler>();
        }

        public T Resolve<T>()
        {
            return _kernel.Get<T>();
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return _kernel.GetAll<T>();
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            return _kernel.GetAll(type);
        }

        public object Resolve(Type type)
        {
            return _kernel.Get(type);
        }

        public void Register<T, TK>() where T : class where TK : T
        {
            _kernel.Unbind<T>();
            _kernel.Bind<T>().To<TK>();
        }

        public void Register<T, TK>(Action<TK> configurationAction) where T : class where TK : T
        {
            _kernel.Unbind<T>();
            _kernel.Bind<T>().To<TK>().OnActivation(configurationAction);
        }

        public void Register<T>(T instance)
        {
            _kernel.Unbind<T>();
            _kernel.Bind<T>().ToConstant(instance);
        }

        public void Dispose()
        {
            _kernel.Dispose();
        }
    }
}