using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;

namespace SeekU.StructureMap
{
    public class SeekUStructureMapResolver : IDependencyResolver
    {
        protected IContainer Container;
        private readonly bool _supperssDisposal;

        public SeekUStructureMapResolver(bool suppressDisposal = true) : this(ObjectFactory.Container)
        {
            _supperssDisposal = suppressDisposal;
        }

        public SeekUStructureMapResolver(IContainer container)
        {
            Container = container;
        }

        public virtual T Resolve<T>()
        {
            try
            {
                return Container.GetInstance<T>();
            }
            catch (StructureMapException)
            {
                return default(T);
            }
        }

        public virtual IEnumerable<T> ResolveAll<T>()
        {
            return Container.GetAllInstances<T>();
        }

        public virtual IEnumerable<object> ResolveAll(Type type)
        {
            var instances = Container.GetAllInstances(type);

            return instances.Cast<object>();
        }

        public virtual object Resolve(Type type)
        {
            try
            {
                return Container.GetInstance(type);
            }
            catch (StructureMapException)
            {
                return null;
            }
        }

        public virtual void Register<T, TK>()
            where T : class
            where TK : T
        {
            Container.Configure(x => x.For<T>().Use<TK>());
        }

        public virtual void Register<T, TK>(Action<TK> configurationAction)
            where T : class
            where TK : T
        {
            Container.Configure(x => x.For<T>().Use<TK>().OnCreation(typeof(TK).FullName, configurationAction));
        }

        public virtual void Register<T>(T instance) where T : class
        {
            Container.Configure(x => x.For<T>().Use(instance));
        }

        public virtual void Dispose()
        {
            if (_supperssDisposal)
            {
                return;
            }

            Container.Dispose();
        }
    }
}
