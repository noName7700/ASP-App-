﻿using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using ASP_App_ПИС.Helpers;

namespace ASP_App_ПИС.Controllers
{
    [Authorize]
    public class ScheduleOneController : Controller
    {
        private IWebService _service;
        private ISort _sort;

        public ScheduleOneController(IWebService service, ISort sort)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _sort = sort;
        }

        // тут изменила - теперь это id cont_loc
        [Route("/scheduleone/{id}")]
        public async Task<IActionResult> Index(int id, string search, string search1, string search2, string sort = "startdate", string dir = "desc", int page = 1)
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            var tasks = (await _service.GetTaskMonth(id, userid)).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                DateTime searchDate = DateTime.Parse(search);
                tasks = tasks.Where(ts => ts.startdate == searchDate).Select(m => m).ToList();
                ViewData["search"] = search;
            }
            if (!string.IsNullOrEmpty(search1))
            {
                if (DateTime.TryParse(search1, out DateTime date))
                {
                    tasks = tasks.Where(ts => ts.enddate == date).Select(m => m).ToList();
                    ViewData["search1"] = search1;
                }
            }
            if (!string.IsNullOrEmpty(search2))
            {
                if (int.TryParse(search2, out int count))
                {
                    tasks = tasks.Where(m => m.countanimal.ToString().Contains(search2, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                    ViewData["search2"] = search2;
                }
            }

            tasks = dir == "asc" ? _sort.SortAsc(tasks, sort) : _sort.SortDesc(tasks, sort);

            ViewData["id"] = id;
            var conloc = await _service.GetOneContract_LocalityFromId(id);
            ViewData["locality"] = conloc.Locality.name;

            int pageSize = 10;
            var tasksForPage = tasks.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageViewModel pageViewModel = new PageViewModel(tasks.Count(), page, pageSize);
            ViewData["pageView"] = pageViewModel;

            return View(tasksForPage);
        }

        [HttpGet]
        [Route("/scheduleone/add/{id}")]
        public IActionResult Add(int id)
        {
            if (Request.Query.TryGetValue("err", out StringValues err))
            {
                ViewData["err"] = err;
            }
            return View();
        }

        [HttpPost]
        [Route("/scheduleone/add/{id}")]
        public async Task<IActionResult> AddPost(int id)
        {
            var startdateForm = DateTime.Parse(Request.Form["startdate"]);
            var enddateForm = DateTime.Parse(Request.Form["enddate"]);
            var countanimalForm = int.Parse(Request.Form["count-animal"]);
            Schedule lastSch = await _service.GetOneScheduleFromLocDate(id);

            TaskMonth tm = new TaskMonth
            {
                startdate = startdateForm,
                enddate = enddateForm,
                countanimal = countanimalForm,
                scheduleid = lastSch.id
            };

            if ((int)_service.AddTaskMonth(tm).Result.StatusCode == StatusCodes.Status403Forbidden)
            {
                var err = await _service.AddTaskMonth(tm).Result.Content.ReadAsStringAsync();
                return RedirectToPage($"/scheduleone/add/{id}", new { err = err });
            }
            TaskMonth lastTm = await _service.GetLastTaskMonth();

            var claims = HttpContext.Request.HttpContext.User.Claims;
            Locality loc = (await _service.GetOneContract_LocalityFromId(id)).Locality;
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            Journal jo = new Journal
            {
                nametable = 1,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = lastTm.id,
                description = $"{loc.name} - {lastSch.dateapproval.ToString("dd.MM.yyyy")}. Добавлена задача на месяц: {startdateForm.ToString("dd.MM.yyyy")} - " +
                $"{enddateForm.ToString("dd.MM.yyyy")} - {countanimalForm}"
            };
            await _service.AddJournal(jo);

            return Redirect($"/scheduleone/{id}");
        }

        [HttpGet]
        [Route("/scheduleone/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (Request.Query.TryGetValue("err", out StringValues err))
            {
                ViewData["err"] = err;
            }
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
            //await _service.EditTaskMonth(id, tm);
            if ((int)_service.EditTaskMonth(id, tm).Result.StatusCode == StatusCodes.Status403Forbidden)
            {
                var err = await _service.EditTaskMonth(id, tm).Result.Content.ReadAsStringAsync();
                return RedirectToPage($"/scheduleone/add/{id}", new { err = err });
            }

            var claims = HttpContext.Request.HttpContext.User.Claims;
            Schedule sched = await _service.GetScheduleFromTaskMonthId(id);
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            Journal jo = new Journal
            {
                nametable = 1,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = id,
                description = $"{sched.Contract_Locality.Locality.name} - {sched.dateapproval.ToString("dd.MM.yyyy")}. Изменена задача на месяц: {startdateForm.ToString("dd.MM.yyyy")} - " +
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
                description = $"{schedule.Contract_Locality.Locality.name} - {schedule.dateapproval.ToString("dd.MM.yyyy")}. Удалена задача на месяц: {task.startdate.ToString("dd.MM.yyyy")} - " +
                $"{task.enddate.ToString("dd.MM.yyyy")} - {task.countanimal}"
            };
            await _service.AddJournal(jo);

            return Redirect($"/scheduleone/{schedule.contract_localityid}");
        }
    }
}
