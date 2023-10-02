using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;

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
            var animals = await _service.GetAnimals(id, date);
            return View(animals);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Route("/animal/add")]
        public IActionResult Add(string s)
        {
            return View();
        }
    }
}
