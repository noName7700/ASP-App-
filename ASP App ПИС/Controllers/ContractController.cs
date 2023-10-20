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
        public async Task<IActionResult> Add()
        {
            var muns = await _service.GetMunicipalities();
            return View(muns);
        }

        [HttpPost]
        [Route("/contract/add")]
        public async Task<IActionResult> AddPost()
        {
            Contract con = new Contract { validityperiod = DateTime.Parse(Request.Form["validityperiod"]),
                dateconclusion = DateTime.Parse(Request.Form["dateconclusion"]), 
                municipalityid = int.Parse(Request.Form["municipality"])};
            await _service.AddContract(con);
            return Redirect("/contract/");
        }

        [HttpGet]
        [Route("/contract/edit/{id}/{munid}")]
        public async Task<IActionResult> Edit(int id, int munid)
        {
            Contract con = await _service.GetContractOne(id);
            ViewData["munid"] = munid;
            return View(con);
        }

        [HttpPost]
        [Route("/contract/edit/{id}/{munid}")]
        public async Task<IActionResult> EditPut(int id, int munid)
        {
            Contract con = new Contract { validityperiod = DateTime.Parse(Request.Form["validityperiod"]),
                dateconclusion = DateTime.Parse(Request.Form["dateconclusion"]), 
                municipalityid = munid};
            await _service.EditContract(id, con);
            return Redirect("/contract/");
        }
    }
}
