using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ASP_App_ПИС.Controllers
{
    [Authorize]
    public class ActController : Controller
    {
        private IWebService _service;

        public ActController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IActionResult> Index(string search, SortState sort = SortState.NameAsc)
        {
            var locs = await _service.GetLocalActs();

            if (!string.IsNullOrEmpty(search))
            {
                locs = locs.Where(m => m.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search"] = search;
            }

            ViewData["NameSort"] = sort == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            locs = sort switch
            {
                SortState.NameAsc => locs.OrderBy(sc => sc.name),
                SortState.NameDesc => locs.OrderByDescending(sc => sc.name)
            };

            return View(locs);
        }

        [Route("/act/{id}")]
        public async Task<IActionResult> ViewActs(int id, string search, SortState sort = SortState.DateAsc)
        {
            var acts = await _service.GetAct(id);

            if (!string.IsNullOrEmpty(search))
            {
                DateTime searchDate = DateTime.Parse(search);
                acts = acts.Where(ac => ac.datecapture == searchDate).Select(m => m).ToList();
                ViewData["search"] = search;
            }

            var localityname = await _service.GetOneLocality(id);
            ViewData["localityname"] = localityname.name;

            ViewData["DateSort"] = sort == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;
            acts = sort switch
            {
                SortState.DateAsc => acts.OrderBy(sc => sc.datecapture),
                SortState.DateDesc => acts.OrderByDescending(sc => sc.datecapture)
            };

            return View(acts);
        }

        [HttpGet]
        [Route("/act/add")]
        public async Task<IActionResult> Add()
        {
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
            // нахожу контракт по нас. пункту
            var conid = (await _service.GetOneContract_Locality(localityid)).contractid;

            ActCapture act = new ActCapture
            {
                datecapture = DateTime.Parse(Request.Form["datecapture"]),
                localityid = localityid,
                contractid = conid
            };
            await _service.AddAct(act);
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

        [HttpGet]
        [Route("/act/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            ActCapture act = await _service.GetOneAct(id);
            ViewData["act"] = act;
            IEnumerable<Locality> locs = await _service.GetLocalities();
            return View(locs);
        }

        [HttpPost]
        [Route("/act/edit/{id}")]
        public async Task<IActionResult> EditPut(int id)
        {
            DateTime date = DateTime.Parse(Request.Form["datecapture"]);
            int locid = int.Parse(Request.Form["locality"]);
            int conid = (await _service.GetOneContract_Locality(locid)).contractid;

            ActCapture act = new ActCapture
            {
                datecapture = date,
                localityid = locid,
                contractid = conid
            };
            await _service.EditAct(id, act);

            var claims = HttpContext.Request.HttpContext.User.Claims;
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
            Locality locNeed = await _service.GetOneLocality(locid);
            Journal jo = new Journal
            {
                nametable = 4,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = locNeed.id,
                description = $"Изменен акт отлова: {locNeed.name} - {date.ToString("dd.MM.yyyy")}"
            };
            await _service.AddJournal(jo);

            return Redirect("/act/");
        }
    }
}
