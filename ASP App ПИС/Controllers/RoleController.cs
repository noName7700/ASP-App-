﻿using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ASP_App_ПИС.Controllers
{
    public class RoleController : Controller
    {
        private IWebService _service;
        public RoleController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search, SortState sort = SortState.NameAsc, int page = 1)
        {
            var roles = await _service.GetRoles();

            if (!string.IsNullOrEmpty(search))
            {
                roles = roles.Where(o => o.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(o => o).ToList();
                ViewData["search"] = search;
            }

            ViewData["NameSort"] = sort == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            roles = sort switch
            {
                SortState.NameAsc => roles.OrderBy(m => m.name),
                SortState.NameDesc => roles.OrderByDescending(m => m.name)
            };

            int pageSize = 10;
            var rolsForPage = roles.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageViewModel pageViewModel = new PageViewModel(roles.Count(), page, pageSize);
            ViewData["pageView"] = pageViewModel;

            return View(rolsForPage);
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
        [Route("/role/add")]
        public async Task<IActionResult> AddPost()
        {
            var name = Request.Form["name"];
            if (name != "")
            {
                Role role = new Role { name = name };
                await _service.AddRole(role);

                var claims = HttpContext.Request.HttpContext.User.Claims;
                Role roleLast = await _service.GetLastRole();
                int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

                Journal jo = new Journal
                {
                    nametable = 8,
                    usercaptureid = userid,
                    datetimechange = DateTime.Now,
                    idobject = roleLast.id,
                    description = $"Добавлена роль: {roleLast.name}"
                };
                await _service.AddJournal(jo);

                return Redirect("/role/");
            }
            return Redirect("/role/");
        }

        [HttpGet]
        [Route("/role/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            Role org = await _service.GetOneRole(id);
            return View(org);
        }

        [HttpPost]
        [Route("/role/edit/{id}")]
        public async Task<IActionResult> EditPut(int id)
        {
            Role rol = new Role
            {
                name = Request.Form["name"]
            };
            await _service.EditRole(id, rol);

            var claims = HttpContext.Request.HttpContext.User.Claims;
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            Journal jo = new Journal
            {
                nametable = 8,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = id,
                description = $"Изменена роль: {Request.Form["name"]}"
            };
            await _service.AddJournal(jo);

            return Redirect("/role/");
        }

        [HttpGet]
        [Route("/role/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            Role rolDel = await _service.GetOneRole(id);
            await _service.DeleteRole(id);

            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            Journal jo = new Journal
            {
                nametable = 8,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = id,
                description = $"Удалена роль: {rolDel.name}"
            };
            await _service.AddJournal(jo);


            return Redirect($"/role/");
        }
    }
}
