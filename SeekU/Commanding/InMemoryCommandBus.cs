using System;

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
        public ICommandResult Send<T>(T command) where T : ICommand
        {
            // Create an instance of the command handler for the command type
            var commandType = typeof (IHandleCommands<>).MakeGenericType(command.GetType());
            var commandHandler = _dependencyResolver.Resolve(commandType);

            if (commandHandler == null)
            {
                throw new NullReferenceException("No command handler found for type " + typeof(T).FullName);
            }

            // Find the handler's "Handle" method
            var method = commandType.GetMethod("Handle");
            
            try
            {
                var context = new CommandContext(_dependencyResolver);

                // Try to invoke the method using the handler context
                //method.Invoke(commandHandler, new object[] { commandHandlingContext, _dependencyResolver });
                var result = method.Invoke(commandHandler, new object[] { context, command });

                return result == null 
                    ? CommandResult.Successful
                    : (ICommandResult)result;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception invoking 'Handle' method on type " + commandHandler.GetType().Name, ex.InnerException);
            }
        }

        public ValidationResult Validate<T>(T command) where T : ICommand
        {
            try
            {
                var validationType = typeof(IValidateCommands<>).MakeGenericType(command.GetType());
                var validationHandler = _dependencyResolver.Resolve(validationType);

                if (validationHandler == null)
                {
                    return ValidationResult.Successful;
                }

                // Find the handler's "Handle" method
                var method = validationType.GetMethod("Validate");

                try
                {
                    var result = method.Invoke(validationHandler, new object[] { command });

                    return (ValidationResult)result;
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception invoking 'Validate' method on type " + validationHandler.GetType().Name, ex.InnerException);
                }
            }
            catch
            {
                return ValidationResult.Successful;
            }
        }
    }
}
