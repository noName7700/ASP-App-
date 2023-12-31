﻿using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ASP_App_ПИС.Models;
using ASP_App_ПИС.Helpers;

namespace ASP_App_ПИС.Controllers
{
    [Authorize]
    public class AnimalController : Controller
    {
        private IWebService _service;
        private ISort _sort;

        public AnimalController(IWebService service, ISort sort)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _sort = sort;
        }

        [HttpGet]
        [Route("/animal/{id}")]
        public async Task<IActionResult> Index(int id, string search, string search1, string search2, string search3,
            string search4, string search5, string search6, string search7, string search8, string sort = "category", string dir = "desc", int page = 1)
        {
            ViewData["id"] = id;
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            var animals = (await _service.GetAnimals(id, userid)).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                animals = animals.Where(m => m.category.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search"] = search;
            }
            if (!string.IsNullOrEmpty(search1))
            {
                animals = animals.Where(m => m.sex.Contains(search1, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search1"] = search1;
            }
            if (!string.IsNullOrEmpty(search2))
            {
                animals = animals.Where(m => m.breed.Contains(search2, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search2"] = search2;
            }
            if (!string.IsNullOrEmpty(search3))
            {
                animals = animals.Where(m => m.size.Contains(search3, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search3"] = search3;
            }
            if (!string.IsNullOrEmpty(search4))
            {
                animals = animals.Where(m => m.wool.Contains(search4, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search4"] = search4;
            }
            if (!string.IsNullOrEmpty(search5))
            {
                animals = animals.Where(m => m.color.Contains(search5, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search5"] = search5;
            }
            if (!string.IsNullOrEmpty(search6))
            {
                animals = animals.Where(m => m.ears.Contains(search6, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search6"] = search6;
            }
            if (!string.IsNullOrEmpty(search7))
            {
                animals = animals.Where(m => m.tail.Contains(search7, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search7"] = search7;
            }
            if (!string.IsNullOrEmpty(search8))
            {
                animals = animals.Where(m => m.specsings.Contains(search8, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search8"] = search8;
            }

            animals = dir == "asc" ? _sort.SortAsc(animals, sort) : _sort.SortDesc(animals, sort);

            // пагинация
            int pageSize = 10; /* размер строк на одну стр */
            var ansForPage = animals.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageViewModel pageViewModel = new PageViewModel(animals.Count(), page, pageSize);
            ViewData["pageView"] = pageViewModel;

            return View(ansForPage);
        }

        [HttpGet]
        [Route("/animal/add/{id}")]
        public IActionResult Add(int id)
        {
            ViewData["id"] = id;
            return View();
        }

        [HttpPost]
        [Route("/animal/add/{id}")]
        public async Task<IActionResult> AddPost(int id)
        {
            Animal animal = new Animal
            {
                breed = Request.Form["breed"],
                wool = Request.Form["wool"],
                category = Request.Form["category"],
                color = Request.Form["color"],
                ears = Request.Form["ears"],
                sex = Request.Form["sex"],
                size = Request.Form["size"],
                tail = Request.Form["tail"],
                specsings = Request.Form["specsigns"],
                actcaptureid = id
            };
            await _service.AddAnimal(animal);
            Animal animalLast = await _service.GetLastAnimal();

            var claims = HttpContext.Request.HttpContext.User.Claims;
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
            Journal jo = new Journal
            {
                nametable = 4,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = animalLast.id,
                description = $"{animalLast.ActCapture.Locality.name} - {animalLast.ActCapture.datecapture.ToString("dd.MM.yyyy")} " +
                $"Добавлено животное: {animalLast.category} - {animalLast.tail} - {animalLast.wool} - " +
                $"{animalLast.sex} - {animalLast.size} - {animalLast.breed} - {animalLast.color} - {animalLast.ears}"
            };
            await _service.AddJournal(jo);

            return Redirect($"/animal/{id}");
        }

        [HttpGet]
        [Route("/animal/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            Animal an = await _service.GetAnimalOne(id);
            return View(an);
        }

        [HttpPost]
        [Route("/animal/edit/{id}")]
        public async Task<IActionResult> EditPut(int id)
        {
            Animal an = new Animal
            {
                breed = Request.Form["breed"],
                wool = Request.Form["wool"],
                category = Request.Form["category"],
                color = Request.Form["color"],
                ears = Request.Form["ears"],
                sex = Request.Form["sex"],
                size = Request.Form["size"],
                tail = Request.Form["tail"],
                specsings = Request.Form["specsigns"],
            };
            await _service.EditAnimal(id, an);

            Animal animalEdit = await _service.GetAnimalOne(id);
            var claims = HttpContext.Request.HttpContext.User.Claims;
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
            Journal jo = new Journal
            {
                nametable = 4,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = animalEdit.id,
                description = $"{animalEdit.ActCapture.Locality.name} - {animalEdit.ActCapture.datecapture.ToString("dd.MM.yyyy")} " +
                $"Изменено животное: {animalEdit.category} - {animalEdit.tail} - {animalEdit.wool} - " +
                $"{animalEdit.sex} - {animalEdit.size} - {animalEdit.breed} - {animalEdit.color} - {animalEdit.ears}"
            };
            await _service.AddJournal(jo);

            return Redirect("/act/");
        }

        [HttpGet]
        [Route("/animal/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Animal animalDelet = await _service.GetAnimalOne(id);
            await _service.DeleteAnimal(id);
            
            var claims = HttpContext.Request.HttpContext.User.Claims;
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
            Journal jo = new Journal
            {
                nametable = 4,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = animalDelet.id,
                description = $"{animalDelet.ActCapture.Locality.name} - {animalDelet.ActCapture.datecapture.ToString("dd.MM.yyyy")} " +
                $"Удалено животное: {animalDelet.category} - {animalDelet.tail} - {animalDelet.wool} - " +
                $"{animalDelet.sex} - {animalDelet.size} - {animalDelet.breed} - {animalDelet.color} - {animalDelet.ears}"
            };
            await _service.AddJournal(jo);

            return Redirect($"/animal/{animalDelet.ActCapture.id}");
        }
    }
}
