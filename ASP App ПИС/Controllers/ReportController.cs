using ASP_App_ПИС.Models;
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
        private IConfiguration _configuration;
        public ReportController(IWebService service, IConfiguration configuration)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _configuration = configuration;
        }

        [HttpGet]
        [Route("/report/register/money")]
        public async Task<IActionResult> RegisterMoney(string search, string search1, SortState sort = SortState.NameAsc, int page = 1)
        {
            var regMoney = await _service.GetRegisterMoney();
            if (!string.IsNullOrEmpty(search))
            {
                regMoney = regMoney.Where(m => m.statuc.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search"] = search;
            }
            if (!string.IsNullOrEmpty(search1))
            {
                if (DateTime.TryParse(search1, out DateTime date))
                {
                    regMoney = regMoney.Where(m => m.datestatus == date).Select(m => m).ToList();
                    ViewData["search1"] = search1;
                }
            }

            ViewData["NameSort"] = sort == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["DateSort"] = sort == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;
            regMoney = sort switch
            {
                SortState.NameAsc => regMoney.OrderBy(j => j.statuc),
                SortState.NameDesc => regMoney.OrderByDescending(j => j.statuc),
                SortState.DateAsc => regMoney.OrderBy(j => j.datestatus),
                SortState.DateDesc => regMoney.OrderByDescending(j => j.datestatus)
            };

            int pageSize = 10;
            var repsForPage = regMoney.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageViewModel pageViewModel = new PageViewModel(regMoney.Count(), page, pageSize);
            ViewData["pageView"] = pageViewModel;

            return View(repsForPage);
        }

        [HttpGet]
        [Route("/report/register/money/post/{id}")]
        public async Task<IActionResult> RegisterMoneyPost(int id) // id - id в таблице Report
        {
            var rep = await _service.GetOneRegisterMoney(id);
            return View(rep);
        }

        [HttpGet]
        [Route("/report/register/schedule")]
        public async Task<IActionResult> RegisterSchedule(string search, string search1, SortState sort = SortState.NameAsc, int page = 1)
        {
            var regSchedule = await _service.GetRegisterSchedule();
            if (!string.IsNullOrEmpty(search))
            {
                regSchedule = regSchedule.Where(m => m.statuc.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search"] = search;
            }
            if (!string.IsNullOrEmpty(search1))
            {
                if (DateTime.TryParse(search1, out DateTime date))
                {
                    regSchedule = regSchedule.Where(m => m.datestatus == date).Select(m => m).ToList();
                    ViewData["search1"] = search1;
                }
            }

            ViewData["NameSort"] = sort == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["DateSort"] = sort == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;
            regSchedule = sort switch
            {
                SortState.NameAsc => regSchedule.OrderBy(j => j.statuc),
                SortState.NameDesc => regSchedule.OrderByDescending(j => j.statuc),
                SortState.DateAsc => regSchedule.OrderBy(j => j.datestatus),
                SortState.DateDesc => regSchedule.OrderByDescending(j => j.datestatus)
            };

            int pageSize = 10;
            var repsForPage = regSchedule.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageViewModel pageViewModel = new PageViewModel(regSchedule.Count(), page, pageSize);
            ViewData["pageView"] = pageViewModel;

            return View(repsForPage);
        }

        [HttpGet]
        [Route("/report/register/schedule/post/{id}")]
        public async Task<IActionResult> RegisterSchedulePost(int id) // id - id в таблице Report
        {
            var rep = await _service.GetOneRegisterMoney(id);
            return View(rep);
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

            Report rep = new Report
            {
                numreport = 1,
                statuc = "Черновик",
                startdate = con.dateconclusion,
                enddate = con.validityperiod,
                localityname = "0",
                summ = priceItog,
                plancount = 0,
                factcount = 0,
                datestatus = DateTime.Now,
                municipalityid = con.municipalityid
            };
            await _service.AddReport(rep);
            return View(priceItog);
        }

        [HttpGet]
        [Route("/report/edit/money/{id}")]
        public async Task<IActionResult> EditMoney(int id)
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            int munid = int.Parse(claims.Where(c => c.Type == ClaimTypes.StateOrProvince).First().Value);
            var isAdmin = bool.Parse(HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value);

            IEnumerable<Contract> contracts;
            if (isAdmin)
            {
                contracts = await _service.GetContracts();
            }
            else
            {
                munid = int.Parse(claims.Where(c => c.Type == ClaimTypes.StateOrProvince).First().Value);
                contracts = await _service.GetContractsFromMunId(munid);
            }
            ViewData["cons"] = contracts;

            var rep = await _service.GetOneRegisterMoney(id);
            return View(rep);
        }

        [HttpPost]
        [Route("/report/edit/money/{id}")]
        public async Task<IActionResult> EditMoneyPut(int id)
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var role = claims.Where(c => c.Type == ClaimTypes.Role).First().Value.ToString();
            var repCur = await _service.GetOneRegisterMoney(id);
            double priceItog; DateTime start; DateTime end; int munid; string status;
            if (repCur.statuc == "Доработка" && role == "Оператор ОМСУ")
            {
                int idContract = int.Parse(Request.Form["contract"]);
                priceItog = await _service.GetReportsMoney(idContract);
                // нахожу контракт
                var con = await _service.GetContractOne(idContract);
                start = con.dateconclusion;
                end = con.validityperiod;
                munid = con.municipalityid;
                status = "Доработка";
            }
            else
            {
                start = repCur.startdate;
                end = repCur.enddate;
                priceItog = repCur.summ;
                munid = repCur.municipalityid;
                status = Request.Form["status"];
            }
            Report rep = new Report
            {
                numreport = 1,
                statuc = status,
                startdate = start,
                enddate = end,
                localityname = "0",
                summ = priceItog,
                plancount = 0,
                factcount = 0,
                datestatus = DateTime.Now,
                municipalityid = munid
            };
            await _service.EditReport(id, rep);

            // тут как-то на почту оператору омсу отправить уведомление что требуется доработка такого-то отчета
            if (Request.Form["status"] == "Доработка")
            {
                // тут нужно отправить сообщение на почту оператору омсу, а в списке userов найти его так, чтобы муниципалитет
                // юзера совпадал с муниципалитетом измененного отчета и отправить ему по почте
            }

            return Redirect("/report/register/money");
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
            ViewData["config"] = _configuration;
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

            Report rep = new Report
            {
                numreport = 2,
                statuc = "Черновик",
                startdate = con.dateconclusion,
                enddate = con.validityperiod,
                localityname = loc.name,
                summ = 0,
                plancount = countAnimals.Keys.First(),
                factcount = countAnimals.Values.First(),
                datestatus = DateTime.Now,
                municipalityid = con.municipalityid
            };
            await _service.AddReport(rep);

            return View(countAnimals);
        }

        [HttpGet]
        [Route("/report/edit/schedule/{id}")]
        public async Task<IActionResult> EditSchedule(int id)
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            int munid = int.Parse(claims.Where(c => c.Type == ClaimTypes.StateOrProvince).First().Value);
            var isAdmin = bool.Parse(HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value);

            IEnumerable<Contract> contracts;IEnumerable<Locality> localities;
            if (isAdmin)
            {
                contracts = await _service.GetContracts();
                localities = await _service.GetLocalities();
            }
            else
            {
                munid = int.Parse(claims.Where(c => c.Type == ClaimTypes.StateOrProvince).First().Value);
                contracts = await _service.GetContractsFromMunId(munid);
                localities = await _service.GetLocalitiesFromMunId(munid);
            }
            ViewData["cons"] = contracts;
            ViewData["locs"] = localities;
            var rep = await _service.GetOneRegisterMoney(id);
            return View(rep);
        }

        [HttpPost]
        [Route("/report/edit/schedule/{id}")]
        public async Task<IActionResult> EditSchedulePut(int id)
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var role = claims.Where(c => c.Type == ClaimTypes.Role).First().Value.ToString();
            var repCur = await _service.GetOneRegisterMoney(id);
            int planCount; int factCount; DateTime start; DateTime end; int munid; string localityname;
            if (repCur.statuc == "Доработка" && role == "Оператор ОМСУ")
            {
                int idContract = int.Parse(Request.Form["contract"]);
                Dictionary<int, int> countAnimals = await _service.GetReportsSchedule(idContract, int.Parse(Request.Form["locality"]));
                // нахожу контракт
                var con = await _service.GetContractOne(idContract);
                start = con.dateconclusion;
                end = con.validityperiod;
                munid = con.municipalityid;
                planCount = countAnimals.Keys.First();
                factCount = countAnimals.Values.First();
                localityname = (await _service.GetOneLocality(int.Parse(Request.Form["locality"]))).name;
            }
            else
            {
                start = repCur.startdate;
                end = repCur.enddate;
                planCount = repCur.plancount;
                factCount = repCur.factcount;
                localityname = repCur.localityname;
                munid = repCur.municipalityid;
            }
            Report rep = new Report
            {
                numreport = 2,
                statuc = Request.Form["status"],
                startdate = start,
                enddate = end,
                localityname = localityname,
                summ = 0,
                plancount = planCount,
                factcount = factCount,
                datestatus = DateTime.Now,
                municipalityid = munid
            };
            await _service.EditReport(id, rep);

            // тут как-то на почту оператору омсу отправить уведомление что требуется доработка такого-то отчета
            if (Request.Form["status"] == "Доработка")
            {
                // тут нужно отправить сообщение на почту оператору омсу, а в списке userов найти его так, чтобы муниципалитет
                // юзера совпадал с муниципалитетом измененного отчета и отправить ему по почте
            }

            return Redirect("/report/register/schedule");
        }

        [HttpGet]
        [Route("/report/schedule/export/{startdate}/{enddate}/{munid}/{locid}/{plan}/{fact}")]
        public async Task<FileStreamResult> ExportSchedule(string startdate, string enddate, int munid, int locid, int plan, int fact)
        {
            return await _service.GetExcelSchedule(startdate, enddate, munid, locid, plan, fact);
        }
    }
}
