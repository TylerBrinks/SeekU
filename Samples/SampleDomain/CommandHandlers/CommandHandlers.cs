using SampleDomain.Commands;
using SampleDomain.Domain;
using SeekU.Commanding;

namespace SampleDomain.CommandHandlers
{
    public class AccountHandler :
        IHandleCommands<CreateNewAccountCommand>,
        IHandleCommands<DebitAccountCommand>,
        IHandleCommands<CreditAccountCommand>
    {
        public ICommandResult Handle(CommandContext context, CreateNewAccountCommand command)
        {
            var account = new BankAccount(command.Id, command.StartingBalance);

            // This is basically a unit of work.  Persist the event stream.
            context.Finalize(account);

            return CommandResult.Successful;
        }

        public ICommandResult Handle(CommandContext context, DebitAccountCommand command)
        {
            var account = context.GetById<BankAccount>(command.Id);
            account.DebitAccount(command.Amount);

            context.Finalize(account);
            
            return CommandResult.Successful;
        }

        public ICommandResult Handle(CommandContext context, CreditAccountCommand command)
        {
            var account = context.GetById<BankAccount>(command.Id);
            account.CreditAccount(command.Amount);

            context.Finalize(account);
           
            return CommandResult.Successful;
        }
    }
}
