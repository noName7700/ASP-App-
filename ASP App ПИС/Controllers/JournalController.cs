using ASP_App_ПИС.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ASP_App_ПИС.Controllers
{
    public class JournalController : Controller
    {
        private IWebService _service;

        public JournalController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [Route("/journal/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var jous = await _service.GetJournal(id);
            return View(jous);
        }
    }
}
