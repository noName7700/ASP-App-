using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;

namespace ASP_App_ПИС.Controllers
{
    public class ScheduleOneController : Controller
    {
        private IWebService _service;

        public ScheduleOneController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Route("/scheduleone/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var tasks = await _service.GetTaskMonth(id);
            ViewData["id"] = id;
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
            Schedule sch = new Schedule(id, lastTm.id, lastSch.dateapproval);
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

        /*[HttpGet]
        [Route("/scheduleone/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            
        }*/
    }
}
