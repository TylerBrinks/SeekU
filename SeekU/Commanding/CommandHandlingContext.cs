using System.Diagnostics;

namespace SeekU.Commanding
{
    /// <summary>
    /// Context for handling a command of type T
    /// </summary>
    /// <typeparam name="T">Type of command being handled</typeparam>
    internal class CommandHandlingContext<T> : ICommandHandlingContext<T> where T : ICommand
    {
        [DebuggerStepThrough]
        public CommandHandlingContext(T command)
        {
            Command = command;
        }

        public T Command { get; private set; }

        //public int ReturnValue { get; private set; }

        //public void Return(int value)
        //{
        //    ReturnValue = value;
        //}
    }
}