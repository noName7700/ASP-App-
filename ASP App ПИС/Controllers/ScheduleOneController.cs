﻿using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
            Schedule lastSch = await _service.GetLastSchedule(id);
            Schedule sch = new Schedule { localityid = id, taskmonthid = lastTm.id, dateapproval = lastSch.dateapproval };
            await _service.AddSchedule(sch);

            var claims = HttpContext.Request.HttpContext.User.Claims;
            Schedule lastSched = await _service.GetLastSchedule(id);
            Locality loc = await _service.GetOneLocality(id);
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);


            // вместе со всем добавить строку в журнал
            Journal jo = new Journal
            {
                nametable = 1,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = lastSched.id,
                description = $"{loc.name} - {lastSched.dateapproval.ToString("dd.MM.yyyy")}. Добавлена задача на месяц: {startdateForm.ToString("dd.MM.yyyy")} - " +
                $"{enddateForm.ToString("dd.MM.yyyy")} - {countanimalForm}"
            };
            await _service.AddJournal(jo);

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
            var startdateForm = DateTime.Parse(Request.Form["startdate"]);
            var enddateForm = DateTime.Parse(Request.Form["enddate"]);
            var countanimalForm = int.Parse(Request.Form["count-animal"]);
            TaskMonth tm = new TaskMonth
            {
                startdate = startdateForm,
                enddate = enddateForm,
                countanimal = countanimalForm
            };
            await _service.EditTaskMonth(id, tm);

            var claims = HttpContext.Request.HttpContext.User.Claims;
            Schedule sched = await _service.GetScheduleFromTaskMonthId(id);
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            Journal jo = new Journal
            {
                nametable = 1,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = id,
                description = $"{sched.Locality.name} - {sched.dateapproval.ToString("dd.MM.yyyy")}. Изменена задача на месяц: {startdateForm.ToString("dd.MM.yyyy")} - " +
                $"{enddateForm.ToString("dd.MM.yyyy")} - {countanimalForm}"
            };
            await _service.AddJournal(jo);

            return Redirect("/schedule/");
        }

        [HttpGet]
        [Route("/scheduleone/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Schedule schedule = await _service.GetScheduleFromTaskMonthId(id);
            TaskMonth task = await _service.GetTaskMonthOne(id);

            await _service.DeleteSchedule(schedule.id);
            await _service.DeleteTaskMonth(id);

            var claims = HttpContext.Request.HttpContext.User.Claims;
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            Journal jo = new Journal
            {
                nametable = 1,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = id,
                description = $"{schedule.Locality.name} - {schedule.dateapproval.ToString("dd.MM.yyyy")}. Удалена задача на месяц: {task.startdate.ToString("dd.MM.yyyy")} - " +
                $"{task.enddate.ToString("dd.MM.yyyy")} - {task.countanimal}"
            };
            await _service.AddJournal(jo);

            return Redirect($"/scheduleone/{schedule.localityid}");
        }
    }
}
