using Microsoft.AspNetCore.Mvc;
using ASP_App_ПИС.Services.Interfaces;
using Domain;

namespace ASP_App_ПИС.Controllers
{
    public class MunicipalityController : Controller
    {
        private IWebService _service;

        public MunicipalityController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IActionResult> Index()
        {
            var municipalities = await _service.GetMunicipalities();
            return View(municipalities);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        //[HttpPost]
        //[Route("/municipality/add")]
        //public async Task<IActionResult> AddPost()
        //{
        //    if (Request.Form["name"] != "")
        //    {
        //        MunicipalityName mun = new MunicipalityName(Request.Form["name"]);
        //        await _service.AddMunicipality(mun);
        //        return Redirect("/municipality/");
        //    }
        //    return Redirect("/municipality/");
        //}
    }
}
