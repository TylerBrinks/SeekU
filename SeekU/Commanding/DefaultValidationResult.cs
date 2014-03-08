
using System.Collections.Generic;

namespace SeekU.Commanding
{
    public class DefaultValidationResult : ValidationResult
    {
        public DefaultValidationResult()
        {
            ValidationResults = new List<ValidationResult>();
        }

        public IEnumerable<ValidationResult> ValidationResults { get; private set; }
    }
}
