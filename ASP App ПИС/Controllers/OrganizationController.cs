using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Configuration;
using ASP_App_ПИС.Helpers;

namespace ASP_App_ПИС.Controllers
{
    public class OrganizationController : Controller
    {
        private IWebService _service;
        private IConfiguration _configuration;
        public OrganizationController(IWebService service, IConfiguration configuration)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search, string search1, string search2, string search3, string sort = "name", string dir = "desc", int page = 1)
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
            var orgs = (await _service.GetOrganizations(userid)).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                orgs = orgs.Where(o => o.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(o => o).ToList();
                ViewData["search"] = search;
            }
            if (!string.IsNullOrEmpty(search1))
            {
                orgs = orgs.Where(o => o.telephone.Contains(search1, StringComparison.InvariantCultureIgnoreCase)).Select(o => o).ToList();
                ViewData["search1"] = search1;
            }
            if (!string.IsNullOrEmpty(search2))
            {
                orgs = orgs.Where(o => o.email.Contains(search2, StringComparison.InvariantCultureIgnoreCase)).Select(o => o).ToList();
                ViewData["search2"] = search2;
            }
            if (!string.IsNullOrEmpty(search3))
            {
                orgs = orgs.Where(o => o.Locality.name.Contains(search3, StringComparison.InvariantCultureIgnoreCase)).Select(o => o).ToList();
                ViewData["search3"] = search3;
            }

            orgs = dir == "asc" ? new SortByProp().SortAsc(orgs, sort) : new SortByProp().SortDesc(orgs, sort);

            int pageSize = 10;
            var orgsForPage = orgs.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageViewModel pageViewModel = new PageViewModel(orgs.Count(), page, pageSize);
            ViewData["pageView"] = pageViewModel;

            return View(orgsForPage);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            if (Request.Query.TryGetValue("err", out StringValues err))
            {
                ViewData["err"] = err;
            }
            var isAdmin = bool.Parse(HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value);
            ViewData["locs"] = await _service.GetLocalities();
            ViewData["muns"] = await _service.GetMunicipalities();
            ViewData["config"] = _configuration;
            if (isAdmin)
                return View();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/organization/add")]
        public async Task<IActionResult> AddPost()
        {
            var name = Request.Form["name"];
            var telephone = Request.Form["telephone"];
            var email = Request.Form["email"];
            var locid = int.Parse(Request.Form["locality"]);

            Organization org = new Organization { name = name, telephone = telephone, email = email, localityid = locid };
            //await _service.AddOrganization(org);
            if ((int)_service.AddOrganization(org).Result.StatusCode == StatusCodes.Status403Forbidden)
            {
                var err = await _service.AddOrganization(org).Result.Content.ReadAsStringAsync();
                return RedirectToPage("/organization/add", new { err = err });
            }

            var claims = HttpContext.Request.HttpContext.User.Claims;
            Organization orgLast = await _service.GetLastOrganization();
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            Journal jo = new Journal
            {
                nametable = 5,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = orgLast.id,
                description = $"Добавлена организация: {orgLast.name} - {orgLast.telephone} - {orgLast.email} - {orgLast.Locality.name}"
            };
            await _service.AddJournal(jo);

            return Redirect("/organization/");
        }

        [HttpGet]
        [Route("/organization/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            Organization org = await _service.GetOneOrganization(id);
            return View(org);
        }

        [HttpPost]
        [Route("/organization/edit/{id}")]
        public async Task<IActionResult> EditPut(int id)
        {
            Organization org = new Organization
            {
                name = Request.Form["name"],
                telephone = Request.Form["telephone"],
                email = Request.Form["email"],
                localityid = int.Parse(Request.Form["locality"])
            };
            await _service.EditOrganization(id, org);

            var claims = HttpContext.Request.HttpContext.User.Claims;
            Organization orgEdit = await _service.GetOneOrganization(id);
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            Journal jo = new Journal
            {
                nametable = 5,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = orgEdit.id,
                description = $"Изменена организация: {orgEdit.name} - {orgEdit.telephone} - {orgEdit.email} - {orgEdit.Locality.name}"
            };
            await _service.AddJournal(jo);

            return Redirect("/organization/");
        }

        [HttpGet]
        [Route("/organization/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            Organization orgEdit = await _service.GetOneOrganization(id);
            await _service.DeleteOrganization(id);

            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            Journal jo = new Journal
            {
                nametable = 5,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = orgEdit.id,
                description = $"Удалена организация: {orgEdit.name} - {orgEdit.telephone} - {orgEdit.email}"
            };
            await _service.AddJournal(jo);

            
            return Redirect($"/organization/");
        }
    }
}
