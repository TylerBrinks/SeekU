namespace SeekU.Commanding
{
    /// <summary>
    /// Represents a context for handing commands of type T
    /// </summary>
    /// <typeparam name="T">Type of command for creating a command context</typeparam>
    public interface ICommandHandlingContext<out T> where T : ICommand
    {
        T Command { get; }
        //void Return(int value);
    }
}