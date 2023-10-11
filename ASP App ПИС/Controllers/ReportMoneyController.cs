using ASP_App_ПИС.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace ASP_App_ПИС.Controllers
{
    public class ReportMoney : Controller
    {
        private IWebService _service;
        public ReportMoney(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IActionResult> Index()
        {
            var municipalities = await _service.GetMunicipalities();
            return View(municipalities);
        }

        [HttpPost]
        //[Route($"/reportmoney/add")]
        public async Task<IActionResult> IndexPost()
        {
            DateTime startDate = DateTime.Parse(Request.Form["startdate"]);
            DateTime endDate = DateTime.Parse(Request.Form["enddate"]);
            int munid = int.Parse(Request.Form["municipality"]);
            var priceItog = await _service.GetReportsMoney(startDate, endDate, munid);
            return Add(priceItog);
        }

        //[HttpGet]
        //[Route("/reportmoney/add/{priceItog}")]
        //public async Task<IActionResult> Add(double priceItog)
        //{
        //    return View(priceItog);
        //}

        //[HttpPost]
        //[Route("/reportmoney/add")]
        //public async Task<IActionResult> AddPost()
        //{
        //    DateTime startDate = DateTime.Parse(Request.Form["startdate"]);
        //    DateTime endDate = DateTime.Parse(Request.Form["enddate"]);
        //    int munid = int.Parse(Request.Form["municipality"]);
        //    var priceItog = await _service.GetReportsMoney(startDate, endDate, munid);
        //    return Redirect($"/reportmoney/add/{priceItog}");
        //}
        [HttpGet]
        public IActionResult Add(double priceItog)
        {
            return View(priceItog);
        }

        [HttpPost]
        [Route("/reportmoney/add")]
        public async Task<IActionResult> AddPost()
        {
            DateTime startDate = DateTime.Parse(Request.Form["startdate"]);
            DateTime endDate = DateTime.Parse(Request.Form["enddate"]);
            int munid = int.Parse(Request.Form["municipality"]);
            var priceItog = await _service.GetReportsMoney(startDate, endDate, munid);
            return Redirect("/reportmoney/add");
        }
    }
}
