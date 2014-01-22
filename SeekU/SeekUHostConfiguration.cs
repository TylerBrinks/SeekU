using SeekU.Commanding;
using SeekU.Eventing;

namespace SeekU
{
    /// <summary>
    /// SeekU configuration specifying a type of dependency resolver
    /// </summary>
    /// <typeparam name="T">Type of dependency resolver</typeparam>
    public class SeekUHostConfiguration<T> : SeekUHost where T : IDependencyResolver, new()
    {
        public SeekUHostConfiguration()
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
}