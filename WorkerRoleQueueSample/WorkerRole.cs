using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using SeekU;
using SeekU.Commanding;

namespace WorkerRoleQueueSample
{
    public class WorkerRole : RoleEntryPoint
    {
        private const string QueueName = "EventStream";
        // QueueClient is thread-safe. 
        private QueueClient _client;
        private readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);
        private Host _host;

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");
        
            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            _client.OnMessage(message =>
                {
                    try
                    {
                        // Process the message
                        Trace.WriteLine("Processing Service Bus message: " + message.SequenceNumber.ToString());

                        _host.GetCommandBus().Send(GetBody(message));
                    }
                    catch
                    {
                        // Handle any message processing specific exceptions here
                    }
                });

            _completedEvent.WaitOne();
        }

        public ICommand GetBody(BrokeredMessage brokeredMessage)
        {
            var contentType = brokeredMessage.ContentType;
            var bodyType = Type.GetType(contentType, true);

            var stream = brokeredMessage.GetBody<Stream>();
            var serializer = new DataContractSerializer(bodyType);
            var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max);
            var deserializedBody = serializer.ReadObject(reader);
            return (ICommand)deserializedBody;
        }

        public override bool OnStart()
        {
            var config = new HostConfiguration<SeekUResolver>();
            _host = new Host(config);

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
}
