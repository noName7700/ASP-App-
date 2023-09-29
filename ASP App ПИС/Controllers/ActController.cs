using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;


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
            var acts = await _service.GetActs();
            return View(acts);
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
