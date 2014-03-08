
namespace SeekU.Commanding
{
    /// <summary>
    /// Interface representing a validation handler which validates a command of type T
    /// </summary>
    /// <typeparam name="T">Type of command to validate</typeparam>
    public interface IValidateCommands<in T> where T : ICommand
    {
        ValidationResult Validate(T command);
    }
}