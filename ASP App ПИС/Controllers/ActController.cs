using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;


namespace ASP_App_ПИС.Controllers
{
    public class ActController : Controller
    {
        private IWebService _service;

        public ActController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IActionResult> Index()
        {
            var locs = await _service.GetLocalActs();
            return View(locs);
        }

        [Route("/act/{id}")]
        public async Task<IActionResult> ViewActs(int id)
        {
            var acts = await _service.GetAct(id);
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

            ActCapture act = new ActCapture{
                animalid = lastAni.id,
                datecapture = DateTime.Parse(Request.Form["datecapture"]),
                localityid = int.Parse(Request.Form["locality"])
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
            foreach (var loc in oldActs)
            {
                ActCapture act = new ActCapture
                {
                    animalid = loc.animalid,
                    datecapture = DateTime.Parse(Request.Form["datecapture"]),
                    localityid = int.Parse(Request.Form["locality"])
                };
                await _service.EditAct(loc.id, act);
            }
            return Redirect("/act/");
        }
    }
}
