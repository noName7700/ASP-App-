using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Xml;

namespace ASP_App_ПИС.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private IWebService _service;

        public ScheduleController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IActionResult> Index(string search, SortState sort = SortState.NameAsc)
        {
            var schedules = await _service.GetSchedules();

            if (!string.IsNullOrEmpty(search))
            {
                schedules = schedules.Where(m => m.Locality.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search"] = search;
            }

            ViewData["NameSort"] = sort == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["DateSort"] = sort == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;
            schedules = sort switch
            {
                SortState.NameAsc => schedules.OrderBy(sc => sc.Locality.name),
                SortState.NameDesc => schedules.OrderByDescending(sc => sc.Locality.name),
                SortState.DateAsc => schedules.OrderBy(sc => sc.dateapproval),
                SortState.DateDesc => schedules.OrderByDescending(sc => sc.dateapproval)
            };

            return View(schedules);
        }

        [HttpGet]
        [Route("/schedule/add")]
        public async Task<IActionResult> Add()
        {
            IEnumerable<Locality> locs = await _service.GetLocalities();
            IEnumerable<Municipality> muns = await _service.GetMunicipalities();
            var locs_muns = new Dictionary<IEnumerable<Locality>, IEnumerable<Municipality>> { { locs, muns} };
            return View(locs_muns);
        }

        [HttpPost]
        [Route("/schedule/add")]
        public async Task<IActionResult> AddPost()
        {
            if (Request.Form["dateapproval"] != "")
            {
                var startdateForm = DateTime.Parse(Request.Form["startdate"]);
                var enddateForm = DateTime.Parse(Request.Form["enddate"]);
                var countanimalForm = int.Parse(Request.Form["count-animal"]);

                TaskMonth tm = new TaskMonth
                {
                    startdate = startdateForm,
                    enddate = enddateForm,
                    countanimal = countanimalForm
                };
                await _service.AddTaskMonth(tm);
                TaskMonth lastTm = await _service.GetLastTaskMonth();

                var claims = HttpContext.Request.HttpContext.User.Claims;
                var locId = int.Parse(claims.Where(c => c.Type == ClaimTypes.Locality).First().Value);
                var isAdmin = bool.Parse(HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value);
                int localityid = isAdmin ? int.Parse(Request.Form["locality"]) : locId;
                Locality loc = await _service.GetOneLocality(localityid);
                Schedule sch = new Schedule { localityid = loc.id,taskmonthid = lastTm.id, dateapproval = DateTime.Parse(Request.Form["dateapproval"]) };
                await _service.AddSchedule(sch);

                Schedule lastSched = await _service.GetLastSchedule(loc.id);
                int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

                // вместе со всем добавить строку в журнал
                Journal jo = new Journal
                {
                    nametable = 1,
                    usercaptureid = userid,
                    //Usercapture = (await _service.GetUsers()).ToList().Where(u => u.id == userid).FirstOrDefault(),
                    datetimechange = DateTime.Now,
                    idobject = lastSched.id,
                    description = $"{loc.name} - {lastSched.dateapproval.ToString("yyyy-MM-dd")}. Добавлена задача на месяц: {startdateForm.ToString("yyyy-MM-dd")} - " +
                    $"{enddateForm.ToString("yyyy-MM-dd")} - {countanimalForm}"
                };
                await _service.AddJournal(jo);

                return Redirect("/schedule/");
            }
            return Redirect("/schedule/");
        }
    }
}
