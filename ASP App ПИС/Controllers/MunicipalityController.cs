using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Builder;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;

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
        public async Task<IActionResult> Index(string search, SortState sort = SortState.NameAsc, int page = 1)
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

            int pageSize = 10;
            var munsForPage = municipalities.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageViewModel pageViewModel = new PageViewModel(municipalities.Count(), page, pageSize);
            ViewData["pageView"] = pageViewModel;

            return View(munsForPage);
        }

        [HttpGet]
        public IActionResult Add()
        {
            if (Request.Query.TryGetValue("err", out StringValues err))
            {
                ViewData["err"] = err;
            }
            var isAdmin = bool.Parse( HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value);
            if (isAdmin)
                return View();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/municipality/add")]
        public async Task<IActionResult> AddPost()
        {
            Municipality mun = new Municipality { name = Request.Form["name"] };
            //await _service.AddMunicipality(mun);
            if ((int)_service.AddMunicipality(mun).Result.StatusCode == StatusCodes.Status403Forbidden)
            {
                var err = await _service.AddMunicipality(mun).Result.Content.ReadAsStringAsync();
                return RedirectToPage($"/municipality/add", new { err = err });
            }

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
    }
}
