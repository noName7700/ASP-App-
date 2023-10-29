using ASP_App_ПИС.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_App_ПИС.Controllers
{
    public class UserController : Controller
    {
        private IWebService _service;
        public UserController(IWebService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        
        [HttpGet]
        [Route("/login")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> IndexPost(string returnUrl) // было Task<IResult>
        {
            var users = _service.GetUsers();
            var login = Request.Form["login"];
            var password = Request.Form["password"];
            Usercapture user = users.Result.FirstOrDefault(u => u.login == login && u.password == password);
            if (user == null) return Unauthorized();
            var claims = new List<Claim> { 
                new Claim(ClaimTypes.NameIdentifier, user.login), 
                new Claim(ClaimTypes.Role, user.role),
                new Claim(ClaimTypes.Name, user.name),
                new Claim(ClaimTypes.Surname, user.surname),
                new Claim(ClaimTypes.Locality, user.localityid.ToString()),
                new Claim(ClaimTypes.StateOrProvince, user.municipalityid.ToString())
                };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await Request.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            //return Results.Redirect(returnUrl??"/");
            return Redirect("/home/");
        }

        [HttpGet]
        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await Request.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/login");
        }
    }
}
