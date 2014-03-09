using System.Collections.Generic;

namespace SeekU.Commanding
{
    public class ValidationResult
    {
        public ValidationResult()
        {
            Success = true;
            ValidationMessages = new List<string>();
        }

        public static ValidationResult Successful
        {
            get
            {
                return new ValidationResult();
            }
        }

        public bool Success { get; set; }
        public IEnumerable<string> ValidationMessages { get; private set; }
    }
}
