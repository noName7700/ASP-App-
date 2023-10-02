using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;

namespace ASP_App_ПИС.Controllers
{
    public class ScheduleOneController : Controller
    {
        private IWebService _service;

        public ScheduleOneController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Route("/scheduleone/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var tasks = await _service.GetTaskMonth(id);
            return View(tasks);
        }

        [HttpGet]
        [Route("/scheduleone/add")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Route("/scheduleone/add")]
        public IActionResult Add(string s)
        {

            return View();
        }
    }
}
