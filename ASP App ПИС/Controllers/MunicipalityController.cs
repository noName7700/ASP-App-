using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ASP_App_ПИС.Controllers
{
    [Authorize]
    public class MunicipalityController : Controller
    {
        private IWebService _service;

        public MunicipalityController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search, SortState sort = SortState.NameAsc)
        {
            var municipalities = await _service.GetMunicipalities();

            if (!string.IsNullOrEmpty(search))
            {
                municipalities = municipalities.Where(m => m.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search"] = search;
            }

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
            var isAdmin = bool.Parse( HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value);
            if (isAdmin)
                return View();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/municipality/add")]
        public async Task<IActionResult> AddPost()
        {
            if (Request.Form["name"] != "")
            {
                Municipality mun = new Municipality { name = Request.Form["name"] };
                await _service.AddMunicipality(mun);

                var claims = HttpContext.Request.HttpContext.User.Claims;
                int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
                Municipality munLast = await _service.GetLastMunicipality();
                Journal jo = new Journal
                {
                    nametable = 3,
                    usercaptureid = userid,
                    datetimechange = DateTime.Now,
                    idobject = munLast.id,
                    description = $"Добавлен муниципалитет: {munLast.name}"
                };
                await _service.AddJournal(jo);

                return Redirect("/municipality/");
            }
            return Redirect("/municipality/");
        }
    }
}
