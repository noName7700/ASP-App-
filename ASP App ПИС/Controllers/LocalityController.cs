﻿using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;

namespace ASP_App_ПИС.Controllers
{
    public class LocalityController : Controller
    {
        private IWebService _service;

        public LocalityController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Route("/locality/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var localities = await _service.GetLocalitiesFromMunId(id);
            ViewData["id"] = id;
            return View(localities);
        }

        [HttpGet]
        [Route("/locality/add/{id}")]
        public IActionResult Add(int id)
        {
            return View();
        }

        [HttpPost]
        [Route("/locality/add/{id}")]
        public async Task<IActionResult> AddPost(int id)
        {
            if (Request.Form["name"] != "")
            {
                Locality loc = new Locality(Request.Form["name"], double.Parse(Request.Form["tariph"]));
                await _service.AddLocality(loc);
                Locality lastLoc = await _service.GetLastLocality();
                Municipality_Locality munLoc = new Municipality_Locality(id, lastLoc.id);
                await _service.AddMunLoc(munLoc);
                return Redirect($"/locality/{id}");
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
            Locality loc = new Locality(Request.Form["name"], double.Parse(Request.Form["tariph"]));
            await _service.EditLocality(id, loc);
            return Redirect("/municipality/");
        }

        [HttpGet]
        [Route("/locality/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Municipality_Locality munLoc = await _service.GetMunicipalityFromLocalityId(id);
            await _service.DeleteMunicipality(munLoc.id);
            await _service.DeleteLocality(id);
            return Redirect($"/locality/{munLoc.munid}");
        }
    }
}
