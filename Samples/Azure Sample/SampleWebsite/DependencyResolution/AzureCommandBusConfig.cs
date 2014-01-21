using System.Configuration;
using SeekU.Azure.Commanding;

namespace SampleWebsite.DependencyResolution
{
    public static class AzureCommandBusConfig
    {
        public static void Configure(AzureCommandBus bus)
        {
            bus.ServiceBusConnectionString = ConfigurationManager.AppSettings["SeeUServiceBusConnection"];
        }
    }
}