using Microsoft.AspNetCore.Mvc;
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
            var munname = await _service.GetMunicipalityForId(id);
            ViewData["munname"] = munname.name;
            var tariph = await _service.GetTariphForLocality(id);
            var dict = new Dictionary<IEnumerable<Locality>, IEnumerable<Contract_Locality>> { { localities, tariph } };
            return View(dict);
        }

        [HttpGet]
        [Route("/locality/add/{id}")]
        public IActionResult Add(int id)
        {
            return View(id);
        }

        [HttpPost]
        [Route("/locality/add/{id}")]
        public async Task<IActionResult> AddPost(int id)
        {
            if (Request.Form["name"] != "")
            {
                Locality loc = new Locality { name = Request.Form["name"], municipalityid = id };
                await _service.AddLocality(loc);

                // нахожу номер контракта по муниципалитету
                var conId = await _service.GetContractFromMuniciaplity(id); // номер контракта
                var lastloc = await _service.GetLastLocality();

                Contract_Locality con_loc = new Contract_Locality { contractid = conId, localityid = lastloc.id, tariph = double.Parse(Request.Form["tariph"]) };
                await _service.AddContractLocality(con_loc);

                return Redirect($"/locality/{id}");
            }
            return Redirect($"/locality/{id}");
        }

        [HttpGet]
        [Route("/locality/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            Locality loc = await _service.GetOneLocality(id);
            var con_loc = await _service.GetOneContract_Locality(id);
            ViewData["tariph"] = con_loc.tariph;
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

            // нахожу номер контракта по муниципалитету
            var conId = await _service.GetContractFromMuniciaplity(munid.id); // номер контракта
            var lastloc = await _service.GetLastLocality();
            var con_locid = await _service.GetOneContract_Locality(id); // нахожу один контракт-наспункт

            Contract_Locality con_loc = new Contract_Locality { contractid = conId, localityid = lastloc.id, tariph = double.Parse(Request.Form["tariph"]) };
            await _service.EditTariphLocality(con_locid.id, con_loc);

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
