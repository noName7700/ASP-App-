using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;

namespace ASP_App_ПИС.Controllers
{
    public class LocalityController : Controller
    {
        private IWebService _service;

        public LocalityController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Route("/locality/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var localities = await _service.GetLocalitiesFromMunId(id);
            return View(localities);
        }

        [HttpGet]
        [Route("/locality/add")]
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
