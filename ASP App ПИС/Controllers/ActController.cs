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
            var acts = await _service.GetActs();
            return View(acts);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Route("/act/add")]
        public IActionResult AddPost()
        {
            /*Animal animal = new Animal({
                breed = Request.Form["breed"],
                wool = Request.Form["wool"],
                category = Request.Form["category"],
                color = Request.Form["color"],
                ears = Request.Form["ears"],
                sex = Request.Form["sex"],
                size = Request.Form["size"],
                tail = Request.Form["tail"],
                specsigns = Request.Form["specsigns"],
            });*/
            /*ActCapture act = new ActCapture({
                Animal = animal,
                datecapture = Request.Form["datecapture"],
            });*/
            return View();
        }
    }
}
