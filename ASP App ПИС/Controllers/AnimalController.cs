using Microsoft.AspNetCore.Mvc;

namespace ASP_App_ПИС.Controllers
{
    public class AnimalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
