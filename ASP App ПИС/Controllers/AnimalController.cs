using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;

namespace ASP_App_ПИС.Controllers
{
    public class AnimalController : Controller
    {
        private IWebService _service;

        public AnimalController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [Route("/animal/{id}/{date}")]
        public async Task<IActionResult> Index(int id, string date)
        {
            ViewData["id"] = id;
            ViewData["date"] = date;
            var animals = await _service.GetAnimals(id, date);
            return View(animals);
        }

        [HttpGet]
        [Route("/animal/add/{id}/{date}")]
        public IActionResult Add(int id, string date)
        {
            ViewData["id"] = id;
            ViewData["date"] = date;
            return View();
        }

        [HttpPost]
        [Route("/animal/add/{id}/{date}")]
        public async Task<IActionResult> AddPost(int id, string date)
        {
            Animal animal = new Animal
            {
                breed = Request.Form["breed"],
                wool = Request.Form["wool"],
                category = Request.Form["category"],
                color = Request.Form["color"],
                ears = Request.Form["ears"],
                sex = Request.Form["sex"],
                size = Request.Form["size"],
                tail = Request.Form["tail"],
                specsigns = Request.Form["specsigns"],
            };
            await _service.AddAnimal(animal);
            Animal animalLast = await _service.GetLastAnimal();
            ActCapture act = new ActCapture
            {
                animalid = animalLast.id,
                datecapture = DateTime.Parse(date),
                localityid = id
            };
            await _service.AddAct(act);
            return Redirect($"/animal/{id}/{date}");
        }

        [HttpGet]
        [Route("/animal/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            Animal an = await _service.GetAnimalOne(id);
            return View(an);
        }

        [HttpPost]
        [Route("/animal/edit/{id}")]
        public async Task<IActionResult> EditPut(int id)
        {
            Animal an = new Animal
            {
                breed = Request.Form["breed"],
                wool = Request.Form["wool"],
                category = Request.Form["category"],
                color = Request.Form["color"],
                ears = Request.Form["ears"],
                sex = Request.Form["sex"],
                size = Request.Form["size"],
                tail = Request.Form["tail"],
                specsigns = Request.Form["specsigns"],
            };
            await _service.EditAnimal(id, an);
            return Redirect("/act/");
        }

        [HttpGet]
        [Route("/animal/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            ActCapture act = await _service.GetActFromAnimalId(id);
            await _service.DeleteAct(act.id);
            await _service.DeleteAnimal(id);
            return Redirect($"/animal/{act.localityid}/{act.datecapture.ToShortDateString()}");
        }
    }
}
