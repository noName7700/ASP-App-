using Microsoft.AspNetCore.Mvc;

namespace ASP_App_ПИС.Controllers
{
    public class ActController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
