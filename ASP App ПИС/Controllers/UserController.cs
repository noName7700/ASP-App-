using ASP_App_ПИС.Models;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

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
                new Claim(ClaimTypes.Actor, user.id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.login), 
                new Claim(ClaimTypes.Role, user.Role.name),
                new Claim(ClaimTypes.Name, user.name),
                new Claim(ClaimTypes.Surname, user.surname),
                new Claim(ClaimTypes.Locality, user.localityid.ToString()),
                new Claim(ClaimTypes.StateOrProvince, user.municipalityid.ToString())
                };
            claims.Add(new Claim("IsAdmin", user.isadmin.ToString(), "bool"));
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

        [HttpGet]
        [Route("/user")]
        public new async Task<IActionResult> User(string search, SortState sort = SortState.NameAsc)
        {
            var users = await _service.GetUsers();

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(us => us.Organization.name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(us => us).ToList();
                ViewData["search"] = search;
            }

            ViewData["NameSort"] = sort == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            users = sort switch
            {
                SortState.NameAsc => users.OrderBy(sc => sc.surname),
                SortState.NameDesc => users.OrderByDescending(sc => sc.surname)
            };

            return View(users);
        }

        [HttpGet]
        [Route("/user/add")]
        public new async Task<IActionResult> Add()
        {
            ViewData["muns"] = await _service.GetMunicipalities();
            ViewData["locs"] = await _service.GetLocalities();
            ViewData["orgs"] = await _service.GetOrganizations();
            ViewData["roles"] = await _service.GetRoles();
            return View();
        }

        [HttpPost]
        [Route("/user/add")]
        public new async Task<IActionResult> AddPost()
        {
            var role = await _service.GetOneRole(int.Parse(Request.Form["role"]));
            var isAdmin = role.name == "Админ";
            Usercapture user = new Usercapture
            {
                surname = Request.Form["surname"],
                name = Request.Form["name"],
                patronymic = Request.Form["patronymic"],
                roleid = role.id,
                telephone = Request.Form["telephone"],
                email = Request.Form["email"],
                municipalityid = int.Parse(Request.Form["municipality"]),
                localityid = int.Parse(Request.Form["locality"]),
                organizationid = int.Parse(Request.Form["organization"]),
                login = Request.Form["login"],
                password = Request.Form["password"],
                isadmin = isAdmin
            };
            await _service.AddUser(user);

            var claims = HttpContext.Request.HttpContext.User.Claims;
            Usercapture userAdd = await _service.GetLastUser();
            int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            Journal jo = new Journal
            {
                nametable = 6,
                usercaptureid = userid,
                datetimechange = DateTime.Now,
                idobject = userAdd.id,
                description = $"Добавлен пользователь: {userAdd.surname} - {userAdd.name} - {userAdd.patronymic} - {userAdd.Role.name} - {userAdd.Municipality.name} - " +
                $"{userAdd.Locality.name} - {userAdd.Organization.name} - {userAdd.telephone} - {userAdd.email}"
            };
            await _service.AddJournal(jo);

            return Redirect("/user");
        }

        [HttpGet]
        [Route("/user/profile")]
        public async Task<IActionResult> Profile()
        {
            var claims = HttpContext.Request.HttpContext.User.Claims;
            var userId = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);
            var user = _service.GetUsers().Result.Where(u => u.id == userId).First();
            return View(user);
        }
    }
}
