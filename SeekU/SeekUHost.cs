using SeekU.Commanding;
using SeekU.Eventing;

namespace SeekU
{
    /// <summary>
    /// SeekU configuration
    /// </summary>
    public class SeekUHost
    {
        internal IDependencyResolver DependencyResolver;

        public ICommandBus GetCommandBus()
        {
            return DependencyResolver.Resolve<ICommandBus>();
        }

        public IDependencyResolver GetDependencyResolver()
        {
            return DependencyResolver;
        }

        public IEventStore GetEventStore()
        {
            return DependencyResolver.Resolve<IEventStore>();
        }

        public IEventBus GetEventBus()
        {
            return DependencyResolver.Resolve<IEventBus>();
        }

        public ISnapshotStore GetSnapshotStore()
        {
            return DependencyResolver.Resolve<ISnapshotStore>();
        }
    }
}
