using System;
using System.Configuration;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using SeekU.Commanding;

namespace SeekU.Azure.Commanding
{
    /// <summary>
    /// Azure command bus implementation.  Commands are sent to an Azure Service Bus queue.
    /// </summary>
    public class AzureCommandBus : ICommandBus
    {
        private static bool _queueCreated;
        public static string DefaultConnectionString;
        public string AzureServiceBusConnectionString { get; set; }
        
        public AzureCommandBus()
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

        /// <summary>
        /// Sends a command to an azure service bus queue
        /// </summary>
        /// <typeparam name="T">Type of command to send</typeparam>
        /// <param name="command">Command instance</param>
        public ICommandResult Send<T>(T command) where T : ICommand
        {
            var connection = AzureServiceBusConnectionString ?? DefaultConnectionString;

            if (connection == null)
            {
                throw new ArgumentException(@"Azure command bus connection has not been configured.  Please update config file or Dependency Resolver.");
            }

            CreateQueue(connection);

            SendMessage(command, connection);

            return CommandResult.Successful;
        }

        public ValidationResult Validate<T>(T command) where T : ICommand
        {
            return ValidationResult.Successful;
        }

        /// <summary>
        /// Sends the command
        /// </summary>
        /// <param name="command">Command instance</param>
        /// <param name="connection">Azure service bus connection string</param>
        public virtual void SendMessage(ICommand command, string connection)
        {
            var message = new BrokeredMessage(command) { ContentType = command.GetType().AssemblyQualifiedName };
            var client = QueueClient.CreateFromConnectionString(connection, "Commands");
            client.Send(message);
        }

        /// <summary>
        /// Creates the service bus queue if it does not already exist
        /// </summary>
        /// <param name="connection">Azure service bus connection string</param>
        public virtual void CreateQueue(string connection)
        {
            var manager = NamespaceManager.CreateFromConnectionString(connection);

            // Prevent re-entrancy for every command
            if (!_queueCreated)
            {
                if (!manager.QueueExists("Commands"))
                {
                    manager.CreateQueue(new QueueDescription("Commands"));
                }

                _queueCreated = true;
            }
        }
    }
}
