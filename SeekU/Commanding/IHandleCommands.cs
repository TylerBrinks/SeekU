
namespace SeekU.Commanding
{
    /// <summary>
    /// Interface representing a command handler which handles a command of type T
    /// </summary>
    /// <typeparam name="T">Type of command to handle</typeparam>
    public interface IHandleCommands<in T> where T : ICommand
    {
        //void Handle(ICommandHandlingContext<T> handlingContext, IDependencyResolver dependencyResolver);
        void Handle(CommandContext commandContext, T command);
    }
}