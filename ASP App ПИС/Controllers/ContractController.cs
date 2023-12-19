using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using ASP_App_ПИС.Helpers;

namespace ASP_App_ПИС.Controllers
{
    [Authorize]
    public class ContractController : Controller
    {
        private IWebService _service;
        private IConfiguration _configuration;
        private ISort _sort;

        public ContractController(IWebService service, IConfiguration configuration, ISort sort)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _configuration = configuration;
            _sort = sort;
        }

        public async Task<IActionResult> Index(string search, string search1, string search2, string search3, 
            string sort = "id", string dir = "desc", int page = 1)
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
            // получаю все контракты
            var contracts = (await _service.GetContracts(userid)).ToList();

            // получаю все contract_locality
            var con_loc = await _service.GetContract_Localities();
            ViewData["con_locs"] = con_loc;
            

            if (!string.IsNullOrEmpty(search))
            {
                contracts = contracts.Where(m => m.Municipality.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search"] = search;
            }
            if (!string.IsNullOrEmpty(search1))
            {
                contracts = contracts.Where(m => m.id.ToString().Contains(search1, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search1"] = search1;
            }
            if (!string.IsNullOrEmpty(search2))
            {
                if (DateTime.TryParse(search2, out DateTime date))
                {
                    DateTime searchDate = DateTime.Parse(search2);
                    contracts = contracts.Where(m => m.dateconclusion == searchDate).Select(m => m).ToList();
                    ViewData["search2"] = search2;
                }
            }
            if (!string.IsNullOrEmpty(search3))
            {
                if (DateTime.TryParse(search3, out DateTime date))
                {
                    DateTime searchDate = DateTime.Parse(search3);
                    contracts = contracts.Where(m => m.validityperiod == searchDate).Select(m => m).ToList();
                    ViewData["search3"] = search3;
                }
            }

            contracts = dir == "asc" ? _sort.SortAsc(contracts, sort) : _sort.SortDesc(contracts, sort);
            
            int pageSize = 10;
            var consForPage = contracts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageViewModel pageViewModel = new PageViewModel(contracts.Count(), page, pageSize);
            ViewData["pageView"] = pageViewModel;

            return View(consForPage);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
            if (Request.Query.TryGetValue("err", out StringValues err))
            {
                ViewData["err"] = err;
            }
            var muns = await _service.GetMunicipalities();
            var locs = await _service.GetLocalities();

            var orgInLoc = await _service.GetOrganizations(userid);
            ViewData["locs"] = locs;
            ViewData["config"] = _configuration;
            ViewData["orgs"] = orgInLoc;
            return View(muns);
        }

        [HttpPost]
        [Route("/contract/add")]
        public async Task<IActionResult> AddPost()
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var munId = int.Parse(claims.Where(c => c.Type == ClaimTypes.StateOrProvince).First().Value);
            var isAdmin = bool.Parse(HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value);
            var munidRes = isAdmin ? int.Parse(Request.Form["municipality"]) : munId;
            var userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
            var countContract = (await _service.GetContracts(userid))
                .Where(c => c.municipalityid == munidRes && c.validityperiod >= DateTime.Parse(Request.Form["validityperiod"]))
                .Count();

            Contract con = new Contract
            {
                validityperiod = DateTime.Parse(Request.Form["validityperiod"]),
                dateconclusion = DateTime.Parse(Request.Form["dateconclusion"]),
                municipalityid = munidRes
            };
            //await _service.AddContract(con);

            if ((int)_service.AddContract(con).Result.StatusCode == StatusCodes.Status403Forbidden)
            {
                var err = await _service.AddContract(con).Result.Content.ReadAsStringAsync();
                return RedirectToAction("add", "contract", new { err = err });
            }

            // нахожу последний контракт
            Contract conLast = await _service.GetLastContract();

            // нахожу все нужные нас пункты по id муниципалитета пользователя (для админа как-то по другому)
            var locs = await _service.GetLocalitiesFromMunId(munId);

            // добавляю все записи в contract_locality
            foreach (var loc in locs)
            {
                var a = Request.Form[loc.name];
                Contract_Locality con_loc = new Contract_Locality
                {
                    contractid = conLast.id,
                    localityid = loc.id,
                    tariph = double.Parse(Request.Form[loc.id.ToString()]),
                    organizationid = int.Parse(Request.Form[loc.name.ToString()])
                };
                await _service.AddContractLocality(con_loc);
                
            }

            Journal jo = new Journal
            {
                nametable = 2,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = conLast.id,
                description = $"Добавлен контракт: {conLast.Municipality.name} - {conLast.validityperiod.ToString("dd.MM.yyyy")} - {conLast.dateconclusion.ToString("dd.MM.yyyy")}"
            };
            await _service.AddJournal(jo);

            return Redirect("/contract/");
        }

        [HttpGet]
        [Route("/contract/edit/{id}/{munid}")]
        public async Task<IActionResult> Edit(int id, int munid)
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            Contract con = await _service.GetContractOne(id);
            ViewData["munid"] = munid;
            ViewData["con_locs"] = await _service.GetContract_LocalityFromConId(id, userid);
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

            var claims = HttpContext.Request.HttpContext.User.Claims;
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
            Contract contractEdit = await _service.GetContractOne(id);

            // изменяю записи в contract_locality
            var con_locs = await _service.GetContract_LocalityFromConId(id, userid);
            foreach (var cl in con_locs)
            {
                var locid = cl.localityid;
                Contract_Locality con_loc = new Contract_Locality
                {
                    contractid = contractEdit.id,
                    localityid = locid,
                    tariph = double.Parse(Request.Form[cl.id.ToString()])
                };
                await _service.EditTariphLocality(cl.id, con_loc);
            }

            Journal jo = new Journal
            {
                nametable = 2,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = contractEdit.id,
                description = $"Изменен контракт: {contractEdit.Municipality.name} - {contractEdit.validityperiod.ToString("dd.MM.yyyy")} - {contractEdit.dateconclusion.ToString("dd.MM.yyyy")}"
            };
            await _service.AddJournal(jo);

            return Redirect("/contract/");
        }
    }
}
