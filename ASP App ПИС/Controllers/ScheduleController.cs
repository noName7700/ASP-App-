using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;

namespace ASP_App_ПИС.Controllers
{
    public class ScheduleController : Controller
    {
        private IWebService _service;

        public ScheduleController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IActionResult> Index()
        {
            var schedules = await _service.GetSchedules();
            return View(schedules);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Route("/schedule/add")]
        public IActionResult Add(string s)
        {
            return View();
        }
    }
}
