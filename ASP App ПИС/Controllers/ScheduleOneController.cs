using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using Microsoft.AspNetCore.Authorization;

namespace ASP_App_ПИС.Controllers
{
    [Authorize]
    public class ScheduleOneController : Controller
    {
        private IWebService _service;

        public ScheduleOneController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Route("/scheduleone/{id}")]
        public async Task<IActionResult> Index(int id, string search, SortState sort = SortState.DateAsc)
        {
            var tasks = await _service.GetTaskMonth(id);

            if (!string.IsNullOrEmpty(search))
            {
                DateTime searchDate = DateTime.Parse(search);
                tasks = tasks.Where(ts => ts.startdate == searchDate).Select(m => m).ToList();
                ViewData["search"] = search;
            }

            ViewData["DateSort"] = sort == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;
            tasks = sort switch
            {
                SortState.DateAsc => tasks.OrderBy(sc => sc.startdate),
                SortState.DateDesc => tasks.OrderByDescending(sc => sc.startdate)
            };

            ViewData["id"] = id;
            var loc = await _service.GetOneLocality(id);
            ViewData["locality"] = loc.name;
            return View(tasks);
        }

        [HttpGet]
        [Route("/scheduleone/add/{id}")]
        public IActionResult Add(int id)
        {
            return View();
        }

        [HttpPost]
        [Route("/scheduleone/add/{id}")]
        public async Task<IActionResult> AddPost(int id)
        {
            TaskMonth tm = new TaskMonth
            {
                startdate = DateTime.Parse(Request.Form["startdate"]),
                enddate = DateTime.Parse(Request.Form["enddate"]),
                countanimal = int.Parse(Request.Form["count-animal"])
            };
            await _service.AddTaskMonth(tm);
            TaskMonth lastTm = await _service.GetLastTaskMonth();
            Schedule lastSch = await _service.GetLastSchedule(id);
            Schedule sch = new Schedule { localityid = id, taskmonthid = lastTm.id, dateapproval = lastSch.dateapproval };
            await _service.AddSchedule(sch);
            return Redirect($"/scheduleone/{id}");
        }

        [HttpGet]
        [Route("/scheduleone/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            TaskMonth tm = await _service.GetTaskMonthOne(id);
            return View(tm);
        }

        [HttpPost]
        [Route("/scheduleone/edit/{id}")]
        public async Task<IActionResult> EditPut(int id)
        {
            TaskMonth tm = new TaskMonth
            {
                startdate = DateTime.Parse(Request.Form["startdate"]),
                enddate = DateTime.Parse(Request.Form["enddate"]),
                countanimal = int.Parse(Request.Form["count-animal"])
            };
            await _service.EditTaskMonth(id, tm);
            return Redirect("/schedule/");
        }

        [HttpGet]
        [Route("/scheduleone/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Schedule schedule = await _service.GetScheduleFromTaskMonthId(id);
            await _service.DeleteSchedule(schedule.id);
            await _service.DeleteTaskMonth(id);
            return Redirect($"/scheduleone/{schedule.localityid}");
        }
    }
}
