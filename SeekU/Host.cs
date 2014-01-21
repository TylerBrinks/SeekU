using SeekU.Commanding;
using SeekU.Eventing;

namespace SeekU
{
    /// <summary>
    /// SeekU host object used for storing configuration and resolving providers
    /// </summary>
    public class Host
    {
        private readonly HostConfiguration _configuration;

        public Host(HostConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ICommandBus GetCommandBus()
        {
            return _configuration.DependencyResolver.Resolve<ICommandBus>();
        }

        public IDependencyResolver GetDependencyResolver()
        {
            return _configuration.DependencyResolver;
        }

        public IEventStore GetEventStore()
        {
            return _configuration.DependencyResolver.Resolve<IEventStore>();
        }

        public IEventBus GetEventBus()
        {
            return _configuration.DependencyResolver.Resolve<IEventBus>();
        }

        public ISnapshotStore GetSnapshotStore()
        {
            return _configuration.DependencyResolver.Resolve<ISnapshotStore>();
        }
    }
}