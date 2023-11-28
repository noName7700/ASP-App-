using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Primitives;

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
            // поменять - тут я нахожу все контракты по id пользователя и вывожу их
            // при нажатии на кнопку просмотр открывается страница ViewLocActs на которой населенные пункты по этому контракту (из contract_locality)
            // на ней кнопка просмотр по которой открывается страница ViewActs в которой выводятся акты отлова по этому контракту и нас пункту
            // на ней кнопка просмотр есть, по которой уже открываются животные
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var isAdmin = bool.Parse(HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value);
            
            IEnumerable<Domain.Contract> contracts = new List<Domain.Contract>();

            if (!isAdmin)
            {
                var munId = int.Parse(claims.Where(c => c.Type == ClaimTypes.StateOrProvince).First().Value);
                contracts = await _service.GetContractsFromMunId(munId);
            }
            else
                contracts = await _service.GetContracts();

            if (!string.IsNullOrEmpty(search))
            {
                contracts = contracts.Where(m => m.Municipality.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search"] = search;
            }

            ViewData["NameSort"] = sort == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            contracts = sort switch
            {
                SortState.NameAsc => contracts.OrderBy(sc => sc.Municipality.name),
                SortState.NameDesc => contracts.OrderByDescending(sc => sc.Municipality.name)
            };

            return View(contracts);
        }

        [Route("/act/view/{id}")]
        public async Task<IActionResult> ViewLocActs(int id, string search, SortState sort = SortState.NameAsc)
        {
            // назожу все строки из contract_locality и вывожу
            var conLoc = await _service.GetContract_LocalityFromConId(id);

            if (!string.IsNullOrEmpty(search))
            {
                conLoc = conLoc.Where(m => m.Locality.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search"] = search;
            }

            //ViewData["NameSort"] = sort == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            //conLoc = sort switch
            //{
            //    SortState.NameAsc => conLoc.OrderBy(sc => sc.Locality.name),
            //    SortState.NameDesc => conLoc.OrderByDescending(sc => sc.Locality.name)
            //};

            return View(conLoc);
        }

        [Route("/act/{id}")]
        public async Task<IActionResult> ViewActs(int id, string search, SortState sort = SortState.DateAsc)
        {
            var acts = await _service.GetActsFromConLocId(id);

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
            var conid = (await _service.GetDateContract_LocalityForDate(localityid, DateTime.Parse(Request.Form["datecapture"]).ToString("yyyy-MM-dd"))).contractid;
            var sched = await _service.GetOneScheduleFromLocDate(localityid, DateTime.Parse(Request.Form["datecapture"]).ToString("yyyy-MM-dd"));

            ActCapture act = new ActCapture
            {
                datecapture = DateTime.Parse(Request.Form["datecapture"]),
                localityid = localityid,
                contractid = conid,
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
