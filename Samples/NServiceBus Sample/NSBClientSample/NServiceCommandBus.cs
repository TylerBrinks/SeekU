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

        public void Send<T>(T command) where T : ICommand
        {
            NServiceBus.Send("NSBServerSample", command);
        }
    }
}