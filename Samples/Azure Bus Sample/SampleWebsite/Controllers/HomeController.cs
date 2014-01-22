using System;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Web.Mvc;
using SampleDomain.Commands;
using SampleDomain.Domain;
using SampleWebsite.Model;
using SeekU;
using SeekU.Commanding;

namespace SampleWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICommandBus _bus;
        private int _pollingAttempts = 0;

        public HomeController(ICommandBus bus)
        {
            _bus = bus;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var id = SequentialGuid.NewId();
            _bus.Send(new CreateNewAccountCommand(id, 1000));

            return RedirectToAction("Created", new {id});
        }

        public async Task<ActionResult> Created(Guid id)
        {
            // Commands are processed by a worker role.  It could take time before the read model
            // is in sync.  This polls the database every quarter second to see if the account has been
            // created.  Once it has (or once it times out) the method continues.

            // For the sake of the example, there's no database.  Instead, the model will simply simulate
            // latency to show how polling works.
            var pollInterval = TimeSpan.FromSeconds(0.25);
            var timeout = TimeSpan.FromSeconds(3);

            var account = await AwaitTimer.WaitUntil(() => GetNewlyCreatedBankAccount(id),
                    BankAccountIsValid, pollInterval, timeout);

            return View(account);
        }

        public ActionResult Debit(Guid id)
        {
            _bus.Send(new DebitAccountCommand(id, 55));
            return View("Created", new BankAccountReadModel { Id = id });
        }

        public ActionResult Credit(Guid id)
        {
            _bus.Send(new CreditAccountCommand(id, 100));
            return View("Created", new BankAccountReadModel { Id = id });
        }

        private BankAccountReadModel GetNewlyCreatedBankAccount(Guid accountId)
        {
            if (++_pollingAttempts < 8) // wait 2 seconds
            {
                return null;
            }

            return new BankAccountReadModel {Id = accountId , Balance = 999};
        }

        private bool BankAccountIsValid(BankAccountReadModel account)
        {
            return account != null && account.Balance > 0;
        }
    }

}