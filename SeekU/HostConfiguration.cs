using System;
using SeekU.Commanding;
using SeekU.Eventing;

namespace SeekU
{
    /// <summary>
    /// SeekU configuration
    /// </summary>
    public class HostConfiguration
    {
        internal IDependencyResolver DependencyResolver;
    }
    
    /// <summary>
    /// SeekU configuration specifying a type of dependency resolver
    /// </summary>
    /// <typeparam name="T">Type of dependency resolver</typeparam>
    public class HostConfiguration<T> : HostConfiguration where T : IDependencyResolver, new()
    {
        public HostConfiguration()
        {
            DependencyResolver = new T();

            RegisterDefaultTypes();
        }

        private void RegisterDefaultTypes()
        {
            // Register defaul providers
            DependencyResolver.Register(DependencyResolver);
            ForCommandBus().Use<InMemoryCommandBus>()
                .ForEventStore().Use<InMemoryEventStore>()
                .ForEventBus().Use<InProcessEventBus>()
                .ForSnapshotStore().Use<InMemorySnapshotStore>();
        }

        /// <summary>
        /// Register a type external to SeekU for DI purposes
        /// </summary>
        /// <typeparam name="TK">Instance to register for type T</typeparam>
        /// <returns>Configuration</returns>
        public FluentHostConfiguration<T, TK> For<TK>() where TK : class
        {
            return new FluentHostConfiguration<T, TK>(this, DependencyResolver);
        }

        /// <summary>
        /// Configures the type of command bus to use
        /// </summary>
        /// <returns>Configuration</returns>
        public FluentHostConfiguration<T, ICommandBus> ForCommandBus() 
        {
            return new FluentHostConfiguration<T, ICommandBus>(this, DependencyResolver);
        }
        
        /// <summary>
        /// Configures the type of event store to use
        /// </summary>
        /// <returns>Configuration</returns>
        public FluentHostConfiguration<T, IEventStore> ForEventStore()
        {
            return new FluentHostConfiguration<T, IEventStore>(this, DependencyResolver);
        }
       
        /// <summary>
        /// Configures the type of event bus to use
        /// </summary>
        /// <returns>Configuration</returns>
        public FluentHostConfiguration<T, IEventBus> ForEventBus()
        {
            return new FluentHostConfiguration<T, IEventBus>(this, DependencyResolver);
        }
       
        /// <summary>
        /// Configures the type of snapshot store to use
        /// </summary>
        /// <returns>Configuration</returns>
        public FluentHostConfiguration<T, ISnapshotStore> ForSnapshotStore()
        {
            return new FluentHostConfiguration<T, ISnapshotStore>(this, DependencyResolver);
        }
    }

    /// <summary>
    /// Fluent configuration helper class
    /// </summary>
    /// <typeparam name="TIoC">Depencency resolver type</typeparam>
    /// <typeparam name="T">Type to register</typeparam>
    public class FluentHostConfiguration<TIoC, T>
        where TIoC : IDependencyResolver, new()
        where T : class
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly HostConfiguration<TIoC> _configuration;

        public FluentHostConfiguration(HostConfiguration<TIoC> configuration, IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
            _configuration = configuration;
        }

        /// <summary>
        /// Register a specific instance with the dependency resolver.
        /// </summary>
        /// <param name="instance">Instance to register</param>
        /// <returns>Configuration</returns>
        public HostConfiguration<TIoC> Use(T instance)
        {
            _dependencyResolver.Register(instance);
            return _configuration;
        }
        
        /// <summary>
        /// Registers types with the dependency resolver
        /// </summary>
        /// <typeparam name="TConcrete">Type to resolve</typeparam>
        /// <returns>Configuration</returns>
        public HostConfiguration<TIoC> Use<TConcrete>() where TConcrete : T
        {
            return Use<TConcrete>(instance => { });
        }

        /// <summary>
        /// Registers types with the dependency resolver
        /// </summary>
        /// <typeparam name="TConcrete">Type to resolve</typeparam>
        /// <param name="configurationAction">Configuration action to perform upon object creation</param>
        /// <returns>Configuration</returns>
        public HostConfiguration<TIoC> Use<TConcrete>(Action<TConcrete> configurationAction) where TConcrete : T
        {
            _dependencyResolver.Register<T, TConcrete>(configurationAction);
            return _configuration;
        }
    }
}
