using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;


namespace ASP_App_ПИС.Controllers
{
    public class ContractController : Controller
    {
        private IWebService _service;

        public ContractController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IActionResult> Index()
        {
            var contracts = await _service.GetContracts();
            return View(contracts);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Route("/contract/add")]
        public IActionResult Add(string s)
        {
            return View();
        }
    }
}
