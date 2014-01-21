using System;
using System.Collections;
using System.Runtime.Remoting.Channels;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using SeekU.Commanding;

namespace SeekU.Azure.Commanding
{
    public class AzureCommandBus : ICommandBus
    {
        private static bool _queueCreated;
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        public string ServiceBusConnectionString { get; set; }

        public void Send<T>(T command) where T : ICommand
        {
            if (ServiceBusConnectionString == null)
            {
                throw new ArgumentException(@"Azure blob credentials have not been configured.  Please configure credentials with your Dependency Resolver.");
            }

            var manager = NamespaceManager.CreateFromConnectionString(ServiceBusConnectionString);
            
            // Prevent re-entrancy for every command
            if (!_queueCreated  && !manager.QueueExists("EventStream"))
            {
                manager.CreateQueue(new QueueDescription("EventStream"));
                _queueCreated = true;
            }


            var json = JsonConvert.SerializeObject(command, SerializerSettings);
            var client = QueueClient.CreateFromConnectionString(ServiceBusConnectionString, "EventStream");

            var message = new BrokeredMessage(command) {ContentType = command.GetType().AssemblyQualifiedName};

            client.Send(message);
        }
    }
}
