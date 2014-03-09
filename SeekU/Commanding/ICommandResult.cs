using System.Collections.Generic;

namespace SeekU.Commanding
{
    public interface ICommandResult
    {
        bool Success { get; set; }
        IList<string> Errors { get; }
    }
}