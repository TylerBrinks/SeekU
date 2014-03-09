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

        public ValidationResult(string errorMessage) : this()
        {
            Success = false;
            ValidationMessages.Add(errorMessage);
        }

        public static ValidationResult Successful
        {
            get
            {
                return new ValidationResult();
            }
        }

        public bool Success { get; set; }
        public IList<string> ValidationMessages { get; private set; }
    }
}
