using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;

namespace SeekU.StructureMap
{
    public class SeekUStructureMapResolver : IDependencyResolver
    {
        protected IContainer Container;

        public SeekUStructureMapResolver() : this(ObjectFactory.Container)
        {
        }

        public SeekUStructureMapResolver(IContainer container)
        {
            Container = container;
        }

        public virtual T Resolve<T>()
        {
            return Container.GetInstance<T>();
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
            return Container.GetInstance(type);
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
            Container.Configure(x => x.For<T>().Use<TK>().OnCreation(configurationAction));
        }

        public virtual void Register<T>(T instance)
        {
            Container.Configure(x => x.For<T>().Use(instance));
        }

        public virtual void Dispose()
        {
            Container.Dispose();
        }
    }
}
