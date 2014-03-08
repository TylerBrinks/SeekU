
namespace SeekU.Commanding
{
    public class EmptyCommandResult : ICommandResult
    {
        public EmptyCommandResult()
        {
            Success = true;
        }

        public bool Success { get; set; }
    }
}
