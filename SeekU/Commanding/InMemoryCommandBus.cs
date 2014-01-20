using System;
using System.Diagnostics;

namespace SeekU.Commanding
{
    /// <summary>
    /// The InMemoryCommandBus publishes commands to handlers responsible for 
    /// acting on a given command.
    /// </summary>
    public class InMemoryCommandBus : ICommandBus
    {
        private readonly IDependencyResolver _dependencyResolver;

        /// <summary>
        /// InMemoryCommandBus constructor
        /// </summary>
        /// <param name="dependencyResolver">Object for resolving runtime dependencies</param>
        public InMemoryCommandBus(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        /// <summary>
        /// Sends a command of type T to a command handler of type T
        /// </summary>
        /// <typeparam name="T">Type of incoming command</typeparam>
        /// <param name="command">Command instance to broadcast</param>
        //[DebuggerStepThrough]
        public void Send<T>(T command) where T : ICommand
        {
            // Create an instance of the command handler for the command type
            var commandHandler = _dependencyResolver.Resolve<IHandleCommands<T>>();

            if (commandHandler == null)
            {
                throw new NullReferenceException("No command handler found for type " + typeof(T).FullName);
            }

            // Find the handler's "Handle" method
            var method = typeof(IHandleCommands<>).MakeGenericType(typeof(T)).GetMethod("Handle");
            
            try
            {
                var context = new CommandContext(_dependencyResolver);

                // Try to invoke the method using the handler context
                //method.Invoke(commandHandler, new object[] { commandHandlingContext, _dependencyResolver });
                method.Invoke(commandHandler, new object[] { context, command });
            }
            catch (Exception ex)
            {
                throw new Exception("Exception invoking 'Handle' method on type " + commandHandler.GetType().Name, ex.InnerException);
            }
        }
    }
}
