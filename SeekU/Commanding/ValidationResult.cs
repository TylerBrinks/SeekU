using System;
using System.Collections.Generic;

namespace SeekU.Commanding
{
    public class ValidationResult
    {
        public ValidationResult()
        {
            Success = true;
            ValidationMessages = new List<ValidationMessage>();
        }

        public static ValidationResult Successful
        {
            get
            {
                return new ValidationResult();
            }
        }

        public bool Success { get; set; }
        public IEnumerable<ValidationMessage> ValidationMessages { get; private set; }
    }

    public class ValidationMessage
    {
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
    }
}
