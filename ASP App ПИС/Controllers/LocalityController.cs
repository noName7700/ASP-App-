using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;

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
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Route("/locality/add")]
        public async Task<IActionResult> AddPost() // доделать
        {
            if (Request.Form["name"] != "")
            {
                Locality loc = new Locality(Request.Form["name"], double.Parse(Request.Form["tariph"]));
                await _service.AddLocality(loc);
                return Redirect("/locality/");
            }
            return Redirect("/locality/");
        }
    }
}
