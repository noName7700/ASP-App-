﻿using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;


namespace ASP_App_ПИС.Controllers
{
    public class ContractController : Controller
    {
        private IWebService _service;

        public ContractController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IActionResult> Index(SortState sort = SortState.NameAsc)
        {
            var contracts = (await _service.GetContracts()).OrderBy(c => c.Municipality.name);

            ViewData["NameSort"] = sort == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            contracts = sort switch
            {
                SortState.NameAsc => contracts.OrderBy(sc => sc.Municipality.name),
                SortState.NameDesc => contracts.OrderByDescending(sc => sc.Municipality.name)
            };

            return View(contracts);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var muns = await _service.GetMunicipalities();
            return View(muns);
        }

        [HttpPost]
        [Route("/contract/add")]
        public async Task<IActionResult> AddPost()
        {
            Contract con = new Contract { validityperiod = DateTime.Parse(Request.Form["validityperiod"]),
                dateconclusion = DateTime.Parse(Request.Form["dateconclusion"]), 
                municipalityid = int.Parse(Request.Form["municipality"])};
            await _service.AddContract(con);
            return Redirect("/contract/");
        }

        [HttpGet]
        [Route("/contract/edit/{id}/{munid}")]
        public async Task<IActionResult> Edit(int id, int munid)
        {
            Contract con = await _service.GetContractOne(id);
            ViewData["munid"] = munid;
            return View(con);
        }

        [HttpPost]
        [Route("/contract/edit/{id}/{munid}")]
        public async Task<IActionResult> EditPut(int id, int munid)
        {
            Contract con = new Contract { validityperiod = DateTime.Parse(Request.Form["validityperiod"]),
                dateconclusion = DateTime.Parse(Request.Form["dateconclusion"]), 
                municipalityid = munid};
            await _service.EditContract(id, con);
            return Redirect("/contract/");
        }
    }
}
