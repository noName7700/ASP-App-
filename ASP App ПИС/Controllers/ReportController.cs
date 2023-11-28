using ASP_App_ПИС.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_App_ПИС.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private IWebService _service;
        public ReportController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [Route("/report/money")]
        public async Task<IActionResult> IndexMoney()
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var municipalities = await _service.GetMunicipalities();
            var isAdmin = bool.Parse(HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value);
            int munid; IEnumerable<Contract> contracts;
            if (isAdmin)
            {
                contracts = await _service.GetContracts();
            }
            else
            {
                munid = int.Parse(claims.Where(c => c.Type == ClaimTypes.StateOrProvince).First().Value);
                contracts = await _service.GetContractsFromMunId(munid);
            }
            return View(contracts);
        }

        [HttpPost]
        [Route("/report/money")]
        public async Task<IActionResult> IndexMoneyPost()
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            int munid = int.Parse(claims.Where(c => c.Type == ClaimTypes.StateOrProvince).First().Value);
            ViewData["munid"] = munid;
            //var priceItog = await _service.GetReportsMoney(Request.Form["startdate"], Request.Form["enddate"], munid);
            int idContract = int.Parse(Request.Form["contract"]);
            var priceItog = await _service.GetReportsMoney(idContract);

            // нахожу контракт
            var con = await _service.GetContractOne(idContract);
            ViewData["startdate"] = con.dateconclusion.ToString("D");
            ViewData["enddate"] = con.validityperiod.ToString("D");
            return View(priceItog);
        }

        [HttpGet]
        [Route("/report/money/export/{startdate}/{enddate}/{munid}/{d}")]
        public async Task<FileStreamResult> ExportMoney(string startdate, string enddate, int munid, double d)
        {
            return await _service.GetExcelMoney(startdate, enddate, munid, d);
        }

        [HttpGet]
        [Route("/report/schedule")]
        public async Task<IActionResult> IndexSchedule()
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var municipalities = await _service.GetMunicipalities();
            var isAdmin = bool.Parse(HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value);
            int munid; IEnumerable<Contract> contracts;
            if (isAdmin)
            {
                contracts = await _service.GetContracts();
            }
            else
            {
                munid = int.Parse(claims.Where(c => c.Type == ClaimTypes.StateOrProvince).First().Value);
                contracts = await _service.GetContractsFromMunId(munid);
            }

            IEnumerable<Locality> locs = await _service.GetLocalities();
            ViewData["locs"] = locs;
            return View(contracts);
        }

        [HttpPost]
        [Route("/report/schedule")]
        public async Task<IActionResult> IndexSchedulePost()
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var isAdmin = bool.Parse(HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value);

            var conid = int.Parse(Request.Form["contract"]);
            int locid = isAdmin ? int.Parse(Request.Form["locality"]) : int.Parse(claims.Where(c => c.Type == ClaimTypes.Locality).First().Value);
            Locality loc = await _service.GetOneLocality(locid);
            Contract con = await _service.GetContractOne(conid);
            var countAnimals = await _service.GetReportsSchedule(conid, locid);
            ViewData["startdate"] = con.dateconclusion.ToString("yyyy-MM-dd");
            ViewData["enddate"] = con.validityperiod.ToString("yyyy-MM-dd");
            ViewData["locname"] = loc.name;
            ViewData["locid"] = locid;
            //ViewData["munid"] = loc.Municipality;
            return View(countAnimals);
        }

        [HttpGet]
        [Route("/report/schedule/export/{startdate}/{enddate}/{munid}/{locid}/{plan}/{fact}")]
        public async Task<FileStreamResult> ExportSchedule(string startdate, string enddate, int munid, int locid, int plan, int fact)
        {
            return await _service.GetExcelSchedule(startdate, enddate, munid, locid, plan, fact);
        }
    }
}
