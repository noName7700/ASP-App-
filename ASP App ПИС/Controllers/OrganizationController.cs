using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ASP_App_ПИС.Controllers
{
    public class OrganizationController : Controller
    {
        private IWebService _service;
        public OrganizationController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search, SortState sort = SortState.NameAsc)
        {
            var orgs = await _service.GetOrganizations();

            if (!string.IsNullOrEmpty(search))
            {
                orgs = orgs.Where(o => o.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(o => o).ToList();
                ViewData["search"] = search;
            }

            ViewData["NameSort"] = sort == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            orgs = sort switch
            {
                SortState.NameAsc => orgs.OrderBy(m => m.name),
                SortState.NameDesc => orgs.OrderByDescending(m => m.name)
            };

            return View(orgs);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var isAdmin = bool.Parse(HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value);
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
            if (name != "" && telephone != "" && email != "")
            {
                Organization org = new Organization { name = name, telephone = telephone, email = email };
                await _service.AddOrganization(org);

                var claims = HttpContext.Request.HttpContext.User.Claims;
                Organization orgLast = await _service.GetLastOrganization();
                int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

                Journal jo = new Journal
                {
                    nametable = 5,
                    usercaptureid = userid,
                    datetimechange = DateTime.Now,
                    idobject = orgLast.id,
                    description = $"Добавлена организация: {orgLast.name} - {orgLast.telephone} - {orgLast.email}"
                };
                await _service.AddJournal(jo);

                return Redirect("/organization/");
            }
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
                email = Request.Form["email"]
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
                description = $"Изменена организация: {orgEdit.name} - {orgEdit.telephone} - {orgEdit.email}"
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
