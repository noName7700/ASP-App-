using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database;

namespace ASP_App_ПИС.Controllers
{
    public class ScheduleController : Controller
    {
        ApplicationContext _context;

        public ScheduleController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var schsss = _context.taskmonth.ToList();
            return View(schsss);
        }
    }
}
