namespace SeekU.Commanding
{
    /// <summary>
    /// Represents a message bus from which commands are broadcast to handlers
    /// </summary>
    public interface ICommandBus
    {
        void Send<T>(T command) where T : ICommand;
    }
}