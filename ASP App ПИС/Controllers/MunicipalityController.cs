using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;

namespace ASP_App_ПИС.Controllers
{
    public class MunicipalityController : Controller
    {
        private IWebService _service;

        public MunicipalityController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IActionResult> Index(SortState sort = SortState.NameAsc)
        {
            var municipalities = (await _service.GetMunicipalities()).OrderBy(m => m.name);
            ViewData["NameSort"] = sort == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            municipalities = sort switch
            {
                SortState.NameAsc => municipalities.OrderBy(m => m.name),
                SortState.NameDesc => municipalities.OrderByDescending(m => m.name)
            };
            return View(municipalities);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Route("/municipality/add")]
        public async Task<IActionResult> AddPost()
        {
            if (Request.Form["name"] != "")
            {
                Municipality mun = new Municipality { name = Request.Form["name"] };
                await _service.AddMunicipality(mun);
                return Redirect("/municipality/");
            }
            return Redirect("/municipality/");
        }
    }
}
