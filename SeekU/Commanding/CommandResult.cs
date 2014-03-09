
using System.Collections.Generic;

namespace SeekU.Commanding
{
    public class CommandResult : ICommandResult
    {
        public CommandResult()
        {
            Success = true;
            Errors = new List<string>();
        }

        public CommandResult(string error) : this()
        {
            Success = false;
            Errors.Add(error);
        }

        public static CommandResult Successful
        {
            get
            {
                return new CommandResult();
            }
        }

        public bool Success { get; set; }
        public IList<string> Errors { get; set; }
    }
}
