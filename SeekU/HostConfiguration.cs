using SeekU.Commanding;
using SeekU.Domain;
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
    /// <typeparam name="TIoC"></typeparam>
    /// <typeparam name="T"></typeparam>
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

        public HostConfiguration<TIoC> Use(T instance)
        {
            _dependencyResolver.Register(instance);
            return _configuration;
        }

        /// <summary>
        /// Registers types with the dependency resolver
        /// </summary>
        /// <typeparam name="TConcrete">Type of resolver</typeparam>
        /// <returns>Configuration</returns>
        public HostConfiguration<TIoC> Use<TConcrete>() where TConcrete : T
        {
            _dependencyResolver.Register<T, TConcrete>();
            return _configuration;
        }
    }
}
