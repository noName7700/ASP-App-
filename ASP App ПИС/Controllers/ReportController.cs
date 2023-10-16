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

        [HttpGet]
        [Route("/report/money")]
        public async Task<IActionResult> IndexMoney()
        {
            var municipalities = await _service.GetMunicipalities();
            return View(municipalities);
        }

        [HttpPost]
        [Route("/report/money")]
        public async Task<IActionResult> IndexMoneyPost()
        {
            int munid = int.Parse(Request.Form["municipality"]);
            var priceItog = await _service.GetReportsMoney(Request.Form["startdate"], Request.Form["enddate"], munid);
            ViewData["start"] = Request.Form["startdate"];
            return View(priceItog);
        }

        [HttpGet]
        [Route("/report/money/export")]
        public async Task<FileStreamResult> ExportMoney()
        {
            return await _service.GetExcelMoney();
        }
    }
}
