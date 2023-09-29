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

        public async Task<IActionResult> Index()
        {
            var animals = await _service.GetAnimals();
            return View(animals);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(string s)
        {
            return View();
        }
    }
}
