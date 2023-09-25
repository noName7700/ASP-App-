using Microsoft.AspNetCore.Mvc;
using Server.Application;
using Domain;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScheduleController : ControllerBase
    {
        ApplicationContext _context;

        public ScheduleController(ApplicationContext context)
        {
            _context = context;
        }

        //public IActionResult Index()
        //{
        //    var schsss = _context.taskmonth.ToList();
        //    return View(schsss);
        //}

        [HttpGet(Name = "GetTaskMonth")]
        public IEnumerable<TaskMonth> Get()
        {
            var schsss = _context.taskmonth.ToList();
            return schsss;
        }
    }
}
