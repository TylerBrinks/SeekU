using NServiceBus;
using SeekU.Commanding;
using ICommand = SeekU.Commanding.ICommand;

namespace NSBClientSample
{
    public class NServiceCommandBus : ICommandBus
    {
        public IBus NServiceBus { get; set; }

        public NServiceCommandBus(IBus bus)
        {
            NServiceBus = bus;
        }

        public ICommandResult Send<T>(T command) where T : ICommand
        {
            NServiceBus.Send("NSBServerSample", command);

            return CommandResult.Successful;
        }

        public ValidationResult Validate<T>(T command) where T : ICommand
        {
            //TODO: Implement custom validation
            return ValidationResult.Successful;
        }
    }
}