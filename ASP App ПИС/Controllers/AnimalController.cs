using Microsoft.AspNetCore.Mvc;

namespace ASP_App_ПИС.Controllers
{
    public class AnimalController : Controller
    {
        public IActionResult Index()
        {
            return View();
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
