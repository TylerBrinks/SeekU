using System.Collections.Generic;

namespace SeekU.Commanding
{
    /// <summary>
    /// Represents a message bus from which commands are broadcast to handlers
    /// </summary>
    public interface ICommandBus
    {
        ICommandResult Send<T>(T command) where T : ICommand;
        ValidationResult Validate<T>(T command) where T : ICommand;
    }

    public interface ICommandResult
    {
        bool Success { get; set; }
    }
}