
using SampleDomain.Commands;
using SeekU.Commanding;

namespace InMemorySample.CommandValidators
{
    public class AccountCommandValidator : IValidateCommands<CreateNewAccountCommand>
    {
        public ValidationResult Validate(CreateNewAccountCommand command)
        {
            return ValidationResult.Successful;
        }
    }
}
