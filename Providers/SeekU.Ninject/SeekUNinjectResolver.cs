using System;
using System.Collections.Generic;
using Ninject;

namespace SeekU.Ninject
{
    public class SeekUNinjectResolver : IDependencyResolver
    {
        protected IKernel Kernel;

        public SeekUNinjectResolver() : this(new StandardKernel())
        {
        }

        public SeekUNinjectResolver(IKernel kernel)
        {
            Kernel = kernel;
        }

        public virtual T Resolve<T>()
        {
            return Kernel.Get<T>();
        }

        public virtual IEnumerable<T> ResolveAll<T>()
        {
            return Kernel.GetAll<T>();
        }

        public virtual IEnumerable<object> ResolveAll(Type type)
        {
            return Kernel.GetAll(type);
        }

        public virtual object Resolve(Type type)
        {
            return Kernel.Get(type);
        }

        public virtual void Register<T, TK>()
            where T : class
            where TK : T
        {
            Kernel.Unbind<T>();
            Kernel.Bind<T>().To<TK>();
        }

        public virtual void Register<T, TK>(Action<TK> configurationAction)
            where T : class
            where TK : T
        {
            Kernel.Unbind<T>();
            Kernel.Bind<T>().To<TK>().OnActivation(configurationAction);
        }

        public virtual void Register<T>(T instance) where T : class
        {
            Kernel.Unbind<T>();
            Kernel.Bind<T>().ToConstant(instance);
        }

        public virtual void Dispose()
        {
            Kernel.Dispose();
        }
    }
}
