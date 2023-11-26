using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ASP_App_ПИС.Controllers
{
    [Authorize]
    public class ContractController : Controller
    {
        private IWebService _service;

        public ContractController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IActionResult> Index(string search, SortState sort = SortState.NameAsc)
        {
            // получаю все контракты
            var contracts = await _service.GetContracts();

            // получаю все contract_locality
            var con_loc = await _service.GetContract_Localities();
            ViewData["con_locs"] = con_loc;
            

            if (!string.IsNullOrEmpty(search))
            {
                contracts = contracts.Where(m => m.Municipality.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(m => m).ToList();
                ViewData["search"] = search;
            }

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
            var locs = await _service.GetLocalities();
            ViewData["locs"] = locs;
            return View(muns);
        }

        [HttpPost]
        [Route("/contract/add")]
        public async Task<IActionResult> AddPost()
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var munId = int.Parse(claims.Where(c => c.Type == ClaimTypes.StateOrProvince).First().Value);
            var isAdmin = bool.Parse(HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value);
            Contract con = new Contract
            {
                validityperiod = DateTime.Parse(Request.Form["validityperiod"]),
                dateconclusion = DateTime.Parse(Request.Form["dateconclusion"]),
                municipalityid = isAdmin ? int.Parse(Request.Form["municipality"]) : munId
            };
            await _service.AddContract(con);

            // нахожу последний контракт
            Contract conLast = await _service.GetLastContract();

            // нахожу все нужные нас пункты по id муниципалитета пользователя (для админа как-то по другому)
            var locs = await _service.GetLocalitiesFromMunId(munId);

            // добавляю все записи в contract_locality
            foreach (var loc in locs)
            {
                Contract_Locality con_loc = new Contract_Locality
                {
                    contractid = conLast.id,
                    localityid = loc.id,
                    tariph = double.Parse(Request.Form[loc.id.ToString()])
                };
                await _service.AddContractLocality(con_loc);
            }

            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
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
            Contract con = await _service.GetContractOne(id);
            ViewData["munid"] = munid;
            ViewData["con_locs"] = await _service.GetContract_LocalityFromConId(id);
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
            var con_locs = await _service.GetContract_LocalityFromConId(id);
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
