using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Primitives;
using System.Drawing;
using ASP_App_ПИС.Helpers;

namespace ASP_App_ПИС.Controllers
{
    [Authorize]
    public class ActController : Controller
    {
        private IWebService _service;
        private ISort _sort;

        public ActController(IWebService service, ISort sort)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _sort = sort;
        }

        public async Task<IActionResult> Index(string search, string search1, string search2, string search3, string sort = "id", string dir = "desc", int page = 1)
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            var contracts = (await _service.GetContracts(userid)).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                contracts = contracts.Where(m => m.Municipality.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search"] = search;
            }

            if (!string.IsNullOrEmpty(search1))
            {
                contracts = contracts.Where(m => m.id.ToString().Contains(search1, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search1"] = search1;
            }

            if (!string.IsNullOrEmpty(search2))
            {
                if (DateTime.TryParse(search2, out DateTime date))
                {
                    DateTime searchDate = DateTime.Parse(search2);
                    contracts = contracts.Where(m => m.dateconclusion == searchDate).Select(m => m).ToList();
                    ViewData["search2"] = search2;
                }
            }

            if (!string.IsNullOrEmpty(search3))
            {
                if (DateTime.TryParse(search3, out DateTime date))
                {
                    DateTime searchDate = DateTime.Parse(search3);
                    contracts = contracts.Where(m => m.validityperiod == searchDate).Select(m => m).ToList();
                    ViewData["search3"] = search3;
                }
            }
            
            contracts = dir == "asc" ? _sort.SortAsc(contracts, sort) : _sort.SortDesc(contracts, sort);

            int pageSize = 10;
            var consForPage = contracts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageViewModel pageViewModel = new PageViewModel(contracts.Count(), page, pageSize);
            ViewData["pageView"] = pageViewModel;

            return View(consForPage);
        }

        [Route("/act/view/{id}")]
        public async Task<IActionResult> ViewLocActs(int id, string search, string sort = "Locality", string dir = "desc", int page = 1)
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            var conLoc = (await _service.GetContract_LocalityFromConId(id, userid)).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                conLoc = conLoc.Where(m => m.Locality.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search"] = search;
            }

            conLoc = dir == "asc" ? _sort.SortAsc(conLoc, sort) : _sort.SortDesc(conLoc, sort);

            int pageSize = 10;
            var conlocsForPage = conLoc.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageViewModel pageViewModel = new PageViewModel(conLoc.Count(), page, pageSize);
            ViewData["pageView"] = pageViewModel;

            return View(conlocsForPage);
        }

        [Route("/act/{id}")]
        public async Task<IActionResult> ViewActs(int id, string search, string sort = "datecapture", string dir = "desc", int page = 1)
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            var acts = (await _service.GetActsFromConLocId(id, userid)).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                if (DateTime.TryParse(search, out DateTime date))
                {
                    DateTime searchDate = DateTime.Parse(search);
                    acts = acts.Where(ac => ac.datecapture == searchDate).Select(m => m).ToList();
                    ViewData["search"] = search;
                }
            }

            var conloc = await _service.GetOneContract_LocalityFromId(id);
            var localityname = await _service.GetOneLocality(conloc.localityid);
            ViewData["localityname"] = localityname.name;

            acts = dir == "asc" ? _sort.SortAsc(acts, sort) : _sort.SortDesc(acts, sort);

            int pageSize = 10;
            var actsForPage = acts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageViewModel pageViewModel = new PageViewModel(acts.Count(), page, pageSize);
            ViewData["pageView"] = pageViewModel;

            return View(actsForPage);
        }

        [HttpGet]
        [Route("/act/add")]
        public async Task<IActionResult> Add()
        {
            if (Request.Query.TryGetValue("err", out StringValues err))
            {
                ViewData["err"] = err;
            }
            IEnumerable<Locality> locs = await _service.GetLocalities();
            return View(locs);
        }

        [HttpPost]
        [Route("/act/add")]
        public async Task<IActionResult> AddPost()
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var locId = int.Parse(claims.Where(c => c.Type == ClaimTypes.Locality).First().Value);
            var isAdmin = bool.Parse(HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value);

            // сначала добавляю акт отлова
            int localityid = isAdmin ? int.Parse(Request.Form["locality"]) : locId;
            // нахожу контракт по нас. пункту и дате отлова
            //var conid = (await _service.GetDateContract_LocalityForDate(localityid, DateTime.Parse(Request.Form["datecapture"]).ToString("yyyy-MM-dd"))).contractid;
            var conloc = await _service.GetDateContract_LocalityForDate(localityid, DateTime.Parse(Request.Form["datecapture"]).ToString("yyyy-MM-dd"));
            var sched = await _service.GetOneScheduleFromLocDate(conloc.id);

            ActCapture act = new ActCapture
            {
                datecapture = DateTime.Parse(Request.Form["datecapture"]),
                localityid = localityid,
                contractid = conloc.contractid,
                scheduleid = sched.id
            };
            //await _service.AddAct(act);
            if ((int)_service.AddAct(act).Result.StatusCode == StatusCodes.Status403Forbidden)
            {
                var err = await _service.AddAct(act).Result.Content.ReadAsStringAsync();
                return RedirectToPage("/act/add", new { err = err });
            }


            ActCapture lastAct = await _service.GetLastActCapture();

            Animal animal = new Animal{
                breed = Request.Form["breed"],
                wool = Request.Form["wool"],
                category = Request.Form["category"],
                color = Request.Form["color"],
                ears = Request.Form["ears"],
                sex = Request.Form["sex"],
                size = Request.Form["size"],
                tail = Request.Form["tail"],
                specsings = Request.Form["specsigns"],
                actcaptureid = lastAct.id
            };
            await _service.AddAnimal(animal);
            Animal lastAni = await _service.GetLastAnimal();

            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            Journal jo = new Journal
            {
                nametable = 4,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = lastAct.id,
                description = $"Добавлен акт отлова: {lastAct.Locality.name} - {lastAct.datecapture.ToString("dd.MM.yyyy")}"
            };
            await _service.AddJournal(jo);

            jo = new Journal
            {
                nametable = 4,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = lastAct.id,
                description = $"{lastAct.Locality.name} - {lastAct.datecapture.ToString("dd.MM.yyyy")} Добавлено животное: {lastAni.category} - {lastAni.tail} - {lastAni.wool} - " +
                $"{lastAni.sex} - {lastAni.size} - {lastAni.breed} - {lastAni.color} - {lastAni.ears}"
            };
            await _service.AddJournal(jo);

            return Redirect("/act/");
        }

        //[HttpGet]
        //[Route("/act/edit/{id}")]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    ActCapture act = await _service.GetOneAct(id);
        //    ViewData["act"] = act;
        //    IEnumerable<Locality> locs = await _service.GetLocalities();
        //    return View(locs);
        //}

        //[HttpPost]
        //[Route("/act/edit/{id}")]
        //public async Task<IActionResult> EditPut(int id)
        //{
        //    DateTime date = DateTime.Parse(Request.Form["datecapture"]);
        //    int locid = int.Parse(Request.Form["locality"]);
        //    int conid = (await _service.GetOneContract_Locality(locid)).contractid;

        //    ActCapture act = new ActCapture
        //    {
        //        datecapture = date,
        //        localityid = locid,
        //        contractid = conid
        //    };
        //    await _service.EditAct(id, act);

        //    var claims = HttpContext.Request.HttpContext.User.Claims;
        //    int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
        //    Locality locNeed = await _service.GetOneLocality(locid);
        //    Journal jo = new Journal
        //    {
        //        nametable = 4,
        //        usercaptureid = userid,
        //        datetimechange = DateTime.Now,
        //        idobject = locNeed.id,
        //        description = $"Изменен акт отлова: {locNeed.name} - {date.ToString("dd.MM.yyyy")}"
        //    };
        //    await _service.AddJournal(jo);

        //    return Redirect("/act/");
        //}
    }
}
