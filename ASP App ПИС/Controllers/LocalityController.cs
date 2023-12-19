using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;
using ASP_App_ПИС.Helpers;

namespace ASP_App_ПИС.Controllers
{
    [Authorize]
    public class LocalityController : Controller
    {
        private IWebService _service;
        private ISort _sort;

        public LocalityController(IWebService service, ISort sort)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _sort = sort;
        }

        [Route("/locality/{id}")]
        public async Task<IActionResult> Index(int id, string search, string sort = "name", string dir = "desc", int page = 1)
        {
            var localities = (await _service.GetLocalitiesFromMunId(id)).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                localities = localities.Where(m => m.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search"] = search;
            }

            localities = dir == "asc" ? _sort.SortAsc(localities, sort) : _sort.SortDesc(localities, sort);

            ViewData["id"] = id;
            var munname = await _service.GetMunicipalityForId(id);
            ViewData["munname"] = munname.name;

            // пагинация
            int pageSize = 10; /* размер строк на одну стр */
            var locsForPage = localities.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageViewModel pageViewModel = new PageViewModel(localities.Count(), page, pageSize);
            ViewData["pageView"] = pageViewModel;

            return View(locsForPage);
        }

        [HttpGet]
        [Route("/locality/add/{id}")]
        public IActionResult Add(int id)
        {
            if (Request.Query.TryGetValue("err", out StringValues err))
            {
                ViewData["err"] = err;
            }
            var isAdmin = bool.Parse(HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value);
            if (isAdmin)
                return View(id);
            return Redirect($"/locality/{id}");
        }

        [HttpPost]
        [Route("/locality/add/{id}")]
        public async Task<IActionResult> AddPost(int id)
        {
            Locality loc = new Locality { name = Request.Form["name"], municipalityid = id };
            //await _service.AddLocality(loc);
            if ((int)_service.AddLocality(loc).Result.StatusCode == StatusCodes.Status403Forbidden)
            {
                var err = await _service.AddLocality(loc).Result.Content.ReadAsStringAsync();
                return RedirectToPage($"/locality/add/{id}", new { err = err });
            }

            return Redirect($"/locality/{id}");
        }

        [HttpGet]
        [Route("/locality/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            Locality loc = await _service.GetOneLocality(id);
            return View(loc);
        }

        [HttpPost]
        [Route("/locality/edit/{id}")]
        public async Task<IActionResult> EditPut(int id)
        {
            //нахожу id муниципалитета
            var munid = await _service.GetMunicipalityFromLocalityId(id);

            Locality loc = new Locality{ name = Request.Form["name"], municipalityid = munid.id };
            await _service.EditLocality(id, loc);

            return Redirect("/municipality/");
        }

        [HttpGet]
        [Route("/locality/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteLocality(id);
            return Redirect($"/municipality/");
        }
    }
}
