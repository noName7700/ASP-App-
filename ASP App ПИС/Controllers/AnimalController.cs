using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ASP_App_ПИС.Controllers
{
    [Authorize]
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
                specsings = Request.Form["specsigns"],
            };
            await _service.AddAnimal(animal);
            Animal animalLast = await _service.GetLastAnimal();
            var mun = await _service.GetMunicipalityFromLocalityId(id);
            int conid = await _service.GetContractFromMuniciaplity(mun.id);
            ActCapture act = new ActCapture
            {
                animalid = animalLast.id,
                datecapture = DateTime.Parse(date),
                localityid = id,
                contractid = conid
            };
            await _service.AddAct(act);

            var claims = HttpContext.Request.HttpContext.User.Claims;
            ActCapture lastAct = await _service.GetLastActCapture();
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
            Journal jo = new Journal
            {
                nametable = 4,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = lastAct.id,
                description = $"{lastAct.Locality.name} - {lastAct.datecapture.ToString("dd.MM.yyyy")} Добавлено животное: {animalLast.category} - {animalLast.tail} - {animalLast.wool} - " +
                $"{animalLast.sex} - {animalLast.size} - {animalLast.breed} - {animalLast.color} - {animalLast.ears}"
            };
            await _service.AddJournal(jo);

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
                specsings = Request.Form["specsigns"],
            };
            await _service.EditAnimal(id, an);

            ActCapture act = await _service.GetActFromAnimalId(id);
            Animal animalEdit = await _service.GetAnimalOne(id);
            var claims = HttpContext.Request.HttpContext.User.Claims;
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
            Journal jo = new Journal
            {
                nametable = 4,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = animalEdit.id,
                description = $"{act.Locality.name} - {act.datecapture.ToString("dd.MM.yyyy")} Изменено животное: {animalEdit.category} - {animalEdit.tail} - {animalEdit.wool} - " +
                $"{animalEdit.sex} - {animalEdit.size} - {animalEdit.breed} - {animalEdit.color} - {animalEdit.ears}"
            };
            await _service.AddJournal(jo);

            return Redirect("/act/");
        }

        [HttpGet]
        [Route("/animal/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            ActCapture act = await _service.GetActFromAnimalId(id);
            Animal animalDelet = await _service.GetAnimalOne(id);
            await _service.DeleteAct(act.id);
            await _service.DeleteAnimal(id);
            
            var claims = HttpContext.Request.HttpContext.User.Claims;
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
            Journal jo = new Journal
            {
                nametable = 4,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = animalDelet.id,
                description = $"{act.Locality.name} - {act.datecapture.ToString("dd.MM.yyyy")} Удалено животное: {animalDelet.category} - {animalDelet.tail} - {animalDelet.wool} - " +
                $"{animalDelet.sex} - {animalDelet.size} - {animalDelet.breed} - {animalDelet.color} - {animalDelet.ears}"
            };
            await _service.AddJournal(jo);

            return Redirect($"/animal/{act.localityid}/{act.datecapture.ToShortDateString()}");
        }
    }
}
