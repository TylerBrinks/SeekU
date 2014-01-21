using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using SeekU.Eventing;

namespace SeekU.Azure.Commanding
{
    public class AzureEventBus : IEventBus
    {
        private static bool _queueCreated;

        public static string DefaultConnectionString;
        public string AzureServiceBusConnectionString { get; set; }

        public AzureEventBus()
        {
            #region Default connection string
            try
            {
                DefaultConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"] ??
                                          ConfigurationManager.AppSettings["SeekU.AzureServiceBus.ConnectionString"];
            }
            catch
            {
                // No need to handle missing default value because it can be configured
                // by the DI container.
            }

            #endregion
        }
      
        public void PublishEvent(DomainEvent domainEvent)
        {
            PublishEvents(new []{domainEvent});
        }

        public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
        {
            var connection = AzureServiceBusConnectionString ?? DefaultConnectionString;

            if (connection == null)
            {
                throw new ArgumentException(@"Azure event bus connection has not been configured.  Please update config file or Dependency Resolver.");
            }

            CreateQueue(connection);

            SendMessage(domainEvents.ToArray(), connection);
        }

        public virtual void SendMessage(object events, string connection)
        {
            var message = new BrokeredMessage(events) { ContentType = events.GetType().AssemblyQualifiedName };
            var client = QueueClient.CreateFromConnectionString(connection, "EventStreams");
            client.Send(message);
        }

        public virtual void CreateQueue(string connection)
        {
            var manager = NamespaceManager.CreateFromConnectionString(connection);

            // Prevent re-entrancy for every command
            if (!_queueCreated)
            {
                if (!manager.QueueExists("EventStreams"))
                {
                    manager.CreateQueue(new QueueDescription("EventStreams"));
                }

                _queueCreated = true;
            }
        }
    }
}
