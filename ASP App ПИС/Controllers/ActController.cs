using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Authorization;

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
            };
            await _service.AddAnimal(animal);
            Animal lastAni = await _service.GetLastAnimal();

            int localityid = int.Parse(Request.Form["locality"]);

            // нахожу контракт по нас. пункту
            var conid = (await _service.GetOneContract_Locality(localityid)).contractid;

            ActCapture act = new ActCapture{
                animalid = lastAni.id,
                datecapture = DateTime.Parse(Request.Form["datecapture"]),
                localityid = localityid,
                contractid = conid
            };
            await _service.AddAct(act);
            return Redirect("/act/");
        }

        [HttpGet]
        [Route("/act/edit/{locid}/{date}")]
        public async Task<IActionResult> Edit(int locid, string date)
        {
            IEnumerable<ActCapture> acts = await _service.GetActs(locid, date);
            ViewData["act"] = acts.First();
            IEnumerable<Locality> locs = await _service.GetLocalities();
            return View(locs);
        }

        [HttpPost]
        [Route("/act/edit/{locid}/{date}")]
        public async Task<IActionResult> EditPut(int locid, string date)
        {
            IEnumerable<ActCapture> oldActs = await _service.GetActs(locid, date);

            // нахожу контракт по нас. пункту
            var conid = (await _service.GetOneContract_Locality(locid)).contractid;

            foreach (var loc in oldActs)
            {

                ActCapture act = new ActCapture
                {
                    animalid = loc.animalid,
                    datecapture = DateTime.Parse(Request.Form["datecapture"]),
                    localityid = int.Parse(Request.Form["locality"]),
                    contractid = conid
                };
                await _service.EditAct(loc.id, act);
            }
            return Redirect("/act/");
        }
    }
}
