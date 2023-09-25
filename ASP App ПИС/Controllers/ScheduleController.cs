using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_App_ПИС.Controllers
{
    public class ScheduleController : Controller
    {

        public ScheduleController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
