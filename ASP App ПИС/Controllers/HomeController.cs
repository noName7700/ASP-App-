using ASP_App_ПИС.Models;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ASP_App_ПИС.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IWebService _service;

        public HomeController(ILogger<HomeController> logger, IWebService service)
        {
            _logger = logger;
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IActionResult> Index()
        {
            // тут
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var munId = int.Parse(claims.Where(c => c.Type == ClaimTypes.StateOrProvince).First().Value);
            var role = claims.Where(c => c.Type == ClaimTypes.Role).First().Value.ToString();

            IEnumerable<Report> reps;
            if (role == "Оператор ОМСУ")
                reps = await _service.GetReportFromMun(munId);
            else
                reps = new List<Report>();

            return View(reps);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}