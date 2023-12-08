using ASP_App_ПИС.Models;
using ASP_App_ПИС.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASP_App_ПИС.Controllers
{
    public class UserController : Controller
    {
        private IWebService _service;
        private IConfiguration _configuration;
        public UserController(IWebService service, IConfiguration configuration)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _configuration = configuration;
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
                new Claim(ClaimTypes.StateOrProvince, user.municipalityid.ToString()),
                new Claim(ClaimTypes.Anonymous, user.Organization.id.ToString())
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
        public new async Task<IActionResult> User(string search, string search1, string search2, string search3,
            string search4, string search5, string search6, SortState sort = SortState.NameAsc)
        {
            var users = await _service.GetUsers();

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(us => $"{us.surname} {us.name} {us.patronymic}".Contains(search, StringComparison.InvariantCultureIgnoreCase)).Select(us => us).ToList();
                ViewData["search"] = search;
            }
            if (!string.IsNullOrEmpty(search1))
            {
                users = users.Where(us => us.telephone.Contains(search1, StringComparison.InvariantCultureIgnoreCase)).Select(us => us).ToList();
                ViewData["search1"] = search1;
            }
            if (!string.IsNullOrEmpty(search2))
            {
                users = users.Where(us => us.email.Contains(search2, StringComparison.InvariantCultureIgnoreCase)).Select(us => us).ToList();
                ViewData["search2"] = search2;
            }
            if (!string.IsNullOrEmpty(search3))
            {
                users = users.Where(us => us.Role.name.Contains(search3, StringComparison.InvariantCultureIgnoreCase)).Select(us => us).ToList();
                ViewData["search3"] = search3;
            }
            if (!string.IsNullOrEmpty(search4))
            {
                users = users.Where(us => us.Municipality.name.Contains(search4, StringComparison.InvariantCultureIgnoreCase)).Select(us => us).ToList();
                ViewData["search4"] = search4;
            }
            if (!string.IsNullOrEmpty(search5))
            {
                users = users.Where(us => us.Locality.name.Contains(search5, StringComparison.InvariantCultureIgnoreCase)).Select(us => us).ToList();
                ViewData["search5"] = search5;
            }
            if (!string.IsNullOrEmpty(search6))
            {
                users = users.Where(us => us.Organization.name.Contains(search6, StringComparison.InvariantCultureIgnoreCase)).Select(us => us).ToList();
                ViewData["search6"] = search6;
            }

            ViewData["NameSort"] = sort == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["UserTelSort"] = sort == SortState.UserTelAsc ? SortState.UserTelDesc : SortState.UserTelAsc;
            ViewData["UserEmailSort"] = sort == SortState.UserEmailAsc ? SortState.UserEmailDesc : SortState.UserEmailAsc;
            ViewData["OrgNameSort"] = sort == SortState.OrgNameAsc ? SortState.OrgNameDesc : SortState.OrgNameAsc;
            ViewData["UserRoleSort"] = sort == SortState.UserRoleAsc ? SortState.UserRoleDesc : SortState.UserRoleAsc;
            ViewData["OrgTelSort"] = sort == SortState.OrgTelAsc ? SortState.OrgTelDesc : SortState.OrgTelAsc;
            ViewData["OrgEmailSort"] = sort == SortState.OrgEmailAsc ? SortState.OrgEmailDesc : SortState.OrgEmailAsc;
            users = sort switch
            {
                SortState.NameAsc => users.OrderBy(sc => $"{sc.surname} {sc.name} {sc.patronymic}"),
                SortState.NameDesc => users.OrderByDescending(sc => $"{sc.surname} {sc.name} {sc.patronymic}"),
                SortState.UserTelAsc => users.OrderBy(j => j.telephone),
                SortState.UserTelDesc => users.OrderByDescending(j => j.telephone),
                SortState.UserEmailAsc => users.OrderBy(j => j.email),
                SortState.UserEmailDesc => users.OrderByDescending(j => j.email),
                SortState.OrgNameAsc => users.OrderBy(j => j.Role.name),
                SortState.OrgNameDesc => users.OrderByDescending(j => j.Role.name),
                SortState.UserRoleAsc => users.OrderBy(j => j.Municipality.name),
                SortState.UserRoleDesc => users.OrderByDescending(j => j.Municipality.name),
                SortState.OrgTelAsc => users.OrderBy(j => j.Locality.name),
                SortState.OrgTelDesc => users.OrderByDescending(j => j.Locality.name),
                SortState.OrgEmailAsc => users.OrderBy(j => j.Organization.name),
                SortState.OrgEmailDesc => users.OrderByDescending(j => j.Organization.name)
            };

            return View(users);
        }

        [HttpGet]
        [Route("/user/add")]
        public new async Task<IActionResult> Add()
        {
            if (Request.Query.TryGetValue("err", out StringValues err))
            {
                ViewData["err"] = err;
            }
            ViewData["muns"] = await _service.GetMunicipalities();
            ViewData["locs"] = await _service.GetLocalities();
            ViewData["orgs"] = await _service.GetOrganizations();
            ViewData["roles"] = await _service.GetRoles();
            ViewData["config"] = _configuration;
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
            //await _service.AddUser(user);
            if ((int)_service.AddUser(user).Result.StatusCode == StatusCodes.Status403Forbidden)
            {
                var err = await _service.AddUser(user).Result.Content.ReadAsStringAsync();
                return RedirectToPage("/user/add", new { err = err });
            }

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
        [Route("/user/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (Request.Query.TryGetValue("err", out StringValues err))
            {
                ViewData["err"] = err;
            }
            Usercapture user = await _service.GetOneUser(id);
            return View(user);
        }

        [HttpPost]
        [Route("/user/edit/{id}")]
        public async Task<IActionResult> EditPut(int id)
        {
            var userCur = await _service.GetOneUser(id);

            Usercapture user = new Usercapture
            {
                surname = Request.Form["surname"],
                name = Request.Form["name"],
                patronymic = Request.Form["patronymic"],
                telephone = Request.Form["telephone"],
                email = Request.Form["email"],
                roleid = int.Parse(Request.Form["role"]),
                municipalityid = int.Parse(Request.Form["municipality"]),
                localityid = int.Parse(Request.Form["locality"]),
                organizationid = int.Parse(Request.Form["organization"]),
                login = userCur.login,
                password = userCur.password,
                isadmin = userCur.isadmin
            };

            //await _service.EditTaskMonth(id, tm);
            if ((int)_service.EditUser(id, user).Result.StatusCode == StatusCodes.Status403Forbidden)
            {
                var err = await _service.EditUser(id, user).Result.Content.ReadAsStringAsync();
                return RedirectToPage($"/user/edit/{id}", new { err = err });
            }

            //var claims = HttpContext.Request.HttpContext.User.Claims;
            //Schedule sched = await _service.GetScheduleFromTaskMonthId(id);
            //int userid = int.Parse(claims.Where(c => c.Type == ClaimTypes.Actor).First().Value);

            //Journal jo = new Journal
            //{
            //    nametable = 1,
            //    usercaptureid = userid,
            //    datetimechange = DateTime.Now,
            //    idobject = id,
            //    description = $"{sched.Contract_Locality.Locality.name} - {sched.dateapproval.ToString("dd.MM.yyyy")}. Изменена задача на месяц: {startdateForm.ToString("dd.MM.yyyy")} - " +
            //    $"{enddateForm.ToString("dd.MM.yyyy")} - {countanimalForm}"
            //};
            //await _service.AddJournal(jo);

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
