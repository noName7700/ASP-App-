﻿using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;

namespace ASP_App_ПИС.Controllers
{
    public class ScheduleController : Controller
    {
        private IWebService _service;

        public ScheduleController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IActionResult> Index()
        {
            var schedules = await _service.GetSchedules();
            return View(schedules);
        }

        [HttpGet]
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
                TaskMonth tm = new TaskMonth {
                    startdate = DateTime.Parse(Request.Form["startdate"]),
                    enddate = DateTime.Parse(Request.Form["enddate"]),
                    countanimal = int.Parse(Request.Form["count-animal"])};
                await _service.AddTaskMonth(tm);
                TaskMonth lastTm = await _service.GetLastTaskMonth();
                Locality loc = await _service.GetOneLocality(int.Parse(Request.Form["locality"]));
                Schedule sch = new Schedule { idlocality = loc.id, idtaskmonth = lastTm.id, dateapproval = DateTime.Parse(Request.Form["dateapproval"]) };
                await _service.AddSchedule(sch);
                return Redirect("/schedule/");
            }
            return Redirect("/schedule/");
        }
    }
}
