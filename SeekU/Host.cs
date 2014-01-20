using SeekU.Commanding;

namespace SeekU
{
    /// <summary>
    /// SeekU host object used for storing configuration and creating
    /// command bus instances.
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
    }
}