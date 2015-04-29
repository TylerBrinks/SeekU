using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using SampleDomain.Domain;
using SampleDomain.Events;
using SeekU;
using SeekU.Commanding;
using SeekU.Eventing;
using SeekU.StructureMap;
using StructureMap;
using StructureMap.Graph;

namespace WorkerRoleQueueSample
{
    public class WorkerRole : RoleEntryPoint
    {
        // Change the queue name to change between command handing and event handling
        private const string QueueName = "Commands";
        //private const string QueueName = "EventStreams";

        private QueueClient _client;
        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);
        //private Host _host;
        private SeekUHostConfiguration<SeekUDemoDependencyResolver> _host;

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");
 
            _client.OnMessage(message =>
                {
                    try
                    {
                        Trace.WriteLine("Processing Service Bus message: " + message.SequenceNumber.ToString());
                        _host.GetCommandBus().Send(GetCommandBody(message));

                        // To handle events, use the following:
                        //_host.GetEventBus().PublishEvents(GetEventBody(message));

                        message.Complete();
                    }
                    catch
                    {
                        // Handle any message processing specific exceptions here
                    }
                });

            _completedEvent.WaitOne();
        }

        public ICommand GetCommandBody(BrokeredMessage brokeredMessage)
        {
            var contentType = brokeredMessage.ContentType;
            var bodyType = Type.GetType(contentType, true);

            var stream = brokeredMessage.GetBody<Stream>();
            var serializer = new DataContractSerializer(bodyType);
            var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max);
            var deserializedBody = serializer.ReadObject(reader);
            return (ICommand)deserializedBody;
        }

        public IEnumerable<DomainEvent> GetEventBody(BrokeredMessage brokeredMessage)
        {
            var contentType = brokeredMessage.ContentType;
            var bodyType = Type.GetType(contentType, true);

            var stream = brokeredMessage.GetBody<Stream>();
            var serializer = new DataContractSerializer(bodyType, new[] { typeof(AccountCreatedEvent), typeof(AccountDebitedEvent), typeof(AccountCreditedEvent) });
            var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max);
            var deserializedBody = serializer.ReadObject(reader);
            return (IEnumerable<DomainEvent>)deserializedBody;
        }

        public override bool OnStart()
        {
            _host = new SeekUHostConfiguration<SeekUDemoDependencyResolver>();

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }

            // Initialize the connection to Service Bus Queue
            _client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            _client.Close();
            _completedEvent.Set();
            base.OnStop();
        }
    }

    public class SeekUDemoDependencyResolver : SeekUStructureMapResolver
    {
        public SeekUDemoDependencyResolver()
        {
            Container.Configure(x => x.Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssemblyContainingType<BankAccount>();
                scan.WithDefaultConventions();
                scan.ConnectImplementationsToTypesClosing(typeof(IHandleCommands<>));
                scan.ConnectImplementationsToTypesClosing(typeof(IHandleDomainEvents<>));
            }));

            Container = ObjectFactory.Container;
        }
    }
}
