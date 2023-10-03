using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;


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
        public async Task<IActionResult> AddPost()
        {
            ContractNumber con = new ContractNumber(
                DateTime.Parse(Request.Form["validityperiod"]), 
                DateTime.Parse(Request.Form["dateconclusion"]));
            await _service.AddContract(con);
            return Redirect("/contract/");
        }
    }
}
