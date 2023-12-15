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
                regMoney = regMoney.Where(m => m.Status.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
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
                SortState.NameAsc => regMoney.OrderBy(j => j.Status.name),
                SortState.NameDesc => regMoney.OrderByDescending(j => j.Status.name),
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
                regSchedule = regSchedule.Where(m => m.Status.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
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
                SortState.NameAsc => regSchedule.OrderBy(j => j.Status.name),
                SortState.NameDesc => regSchedule.OrderByDescending(j => j.Status.name),
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
                statusid = 1, // черновик
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

            var lastRep = await _service.GetLastReport();

            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            Journal jo = new Journal
            {
                nametable = 10,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = lastRep.id,
                description = $"Создан отчет о выполнении работы за контракт: {lastRep.Status.name} - {lastRep.datestatus.ToString("dd.MM.yyyy")} - {lastRep.Municipality.name} - " +
                $"{lastRep.startdate.ToString("dd.MM.yyyy")} - {lastRep.enddate.ToString("dd.MM.yyyy")} - {lastRep.summ}"
            };
            await _service.AddJournal(jo);

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

            var stat = await _service.GetStatusesReports();
            ViewData["statuses"] = stat;

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
            double priceItog; DateTime start; DateTime end; int munid; int statusid;
            if (repCur.Status.name == "Доработка" && role == "Оператор ОМСУ")
            {
                int idContract = int.Parse(Request.Form["contract"]);
                priceItog = await _service.GetReportsMoney(idContract);
                // нахожу контракт
                var con = await _service.GetContractOne(idContract);
                start = con.dateconclusion;
                end = con.validityperiod;
                munid = con.municipalityid;
                statusid = 3;

                int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
                var mun = await _service.GetMunicipalityForId(munid);

                Journal jo = new Journal
                {
                    nametable = 10,
                    usercaptureid = userid,
                    datetimechange = DateTime.Now,
                    idobject = id,
                    description = $"Изменены данные отчета №{id}: {mun.name} - {con.dateconclusion.ToString("dd.MM.yyyy")} - {con.validityperiod.ToString("dd.MM.yyyy")} - {priceItog}"
                };
                await _service.AddJournal(jo);
            }
            else
            {
                start = repCur.startdate;
                end = repCur.enddate;
                priceItog = repCur.summ;
                munid = repCur.municipalityid;
                statusid = int.Parse(Request.Form["status"]);

                int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
                var mun = await _service.GetMunicipalityForId(munid);
                var st = await _service.GetOneStatus(statusid);

                Journal jo = new Journal
                {
                    nametable = 10,
                    usercaptureid = userid,
                    datetimechange = DateTime.Now,
                    idobject = id,
                    description = $"Изменен статус отчета №{id}: {st.name}"
                };
                await _service.AddJournal(jo);
            }
            Report rep = new Report
            {
                numreport = 1,
                statusid = statusid,
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
                statusid = 1, // черновик
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

            var lastRep = await _service.GetLastReport();

            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            Journal jo = new Journal
            {
                nametable = 11,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = lastRep.id,
                description = $"Создан отчет о выполнении работы по планам-графикам: {lastRep.Status.name} - {lastRep.datestatus.ToString("dd.MM.yyyy")} - {lastRep.Municipality.name} - " +
                $"{lastRep.startdate.ToString("dd.MM.yyyy")} - {lastRep.enddate.ToString("dd.MM.yyyy")} - {lastRep.plancount} - {lastRep.factcount}"
            };
            await _service.AddJournal(jo);

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

            var stat = await _service.GetStatusesReports();
            ViewData["statuses"] = stat;

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
            int planCount; int factCount; DateTime start; DateTime end; int munid; string localityname; int statusid;
            if (repCur.Status.name == "Доработка" && role == "Оператор ОМСУ")
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
                statusid = 3;

                int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
                var mun = await _service.GetMunicipalityForId(munid);

                Journal jo = new Journal
                {
                    nametable = 11,
                    usercaptureid = userid,
                    datetimechange = DateTime.Now,
                    idobject = id,
                    description = $"Изменены данные отчета №{id}: {mun.name} - {localityname} - {con.dateconclusion.ToString("dd.MM.yyyy")}" +
                    $" - {con.validityperiod.ToString("dd.MM.yyyy")} - {planCount} - {factCount}"
                };
                await _service.AddJournal(jo);
            }
            else
            {
                start = repCur.startdate;
                end = repCur.enddate;
                planCount = repCur.plancount;
                factCount = repCur.factcount;
                localityname = repCur.localityname;
                munid = repCur.municipalityid;
                statusid = int.Parse(Request.Form["status"]);

                int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
                var mun = await _service.GetMunicipalityForId(munid);
                var st = await _service.GetOneStatus(statusid);

                Journal jo = new Journal
                {
                    nametable = 11,
                    usercaptureid = userid,
                    datetimechange = DateTime.Now,
                    idobject = id,
                    description = $"Изменен статус отчета №{id}: {st.name}"
                };
                await _service.AddJournal(jo);
            }
            Report rep = new Report
            {
                numreport = 2,
                statusid = statusid,
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
