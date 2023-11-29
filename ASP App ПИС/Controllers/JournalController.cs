using ASP_App_ПИС.Models;
using ASP_App_ПИС.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ASP_App_ПИС.Controllers
{
    public class JournalController : Controller
    {
        private IWebService _service;
        private IConfiguration _configuration;

        public JournalController(IWebService service, IConfiguration configuration)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _configuration = configuration;
        }

        [HttpGet]
        [Route("/journal/{id}")]
        public async Task<IActionResult> Index(int id, string search, string search1, string search2, string search3, string search4,
            string search5, string search6, string search7, string search8, string search9, SortState sort = SortState.NameAsc)
        {
            var jous = await _service.GetJournal(id);

            if (!string.IsNullOrEmpty(search))
            {
                jous = jous
                    .Where(j => (j.Usercapture.surname + " " + j.Usercapture.name + " " + j.Usercapture.patronymic).Contains(search, StringComparison.InvariantCultureIgnoreCase))
                    .Select(j => j)
                    .ToList();
                ViewData["search"] = search;
            }
            if (!string.IsNullOrEmpty(search1))
            {
                jous = jous
                    .Where(j => j.Usercapture.telephone.Contains(search1, StringComparison.InvariantCultureIgnoreCase))
                    .Select(j => j)
                    .ToList();
                ViewData["search1"] = search1;
            }
            if (!string.IsNullOrEmpty(search2))
            {
                jous = jous
                    .Where(j => j.Usercapture.email.Contains(search2, StringComparison.InvariantCultureIgnoreCase))
                    .Select(j => j)
                    .ToList();
                ViewData["search2"] = search2;
            }
            if (!string.IsNullOrEmpty(search3))
            {
                jous = jous
                    .Where(j => j.Usercapture.Organization.name.Contains(search3, StringComparison.InvariantCultureIgnoreCase))
                    .Select(j => j)
                    .ToList();
                ViewData["search3"] = search3;
            }
            if (!string.IsNullOrEmpty(search4))
            {
                jous = jous
                    .Where(j => j.Usercapture.Role.name.Contains(search4, StringComparison.InvariantCultureIgnoreCase))
                    .Select(j => j)
                    .ToList();
                ViewData["search4"] = search4;
            }
            if (!string.IsNullOrEmpty(search5))
            {
                jous = jous
                    .Where(j => j.Usercapture.Organization.telephone.Contains(search5, StringComparison.InvariantCultureIgnoreCase))
                    .Select(j => j)
                    .ToList();
                ViewData["search5"] = search5;
            }
            if (!string.IsNullOrEmpty(search5))
            {
                jous = jous
                    .Where(j => j.Usercapture.Organization.telephone.Contains(search5, StringComparison.InvariantCultureIgnoreCase))
                    .Select(j => j)
                    .ToList();
                ViewData["search5"] = search5;
            }
            if (!string.IsNullOrEmpty(search6))
            {
                jous = jous
                    .Where(j => j.Usercapture.Organization.email.Contains(search6, StringComparison.InvariantCultureIgnoreCase))
                    .Select(j => j)
                    .ToList();
                ViewData["search6"] = search6;
            }
            if (!string.IsNullOrEmpty(search7))
            {
                DateTime searchDate = DateTime.Parse(search7);
                jous = jous
                    .Where(j => j.datetimechange == searchDate)
                    .Select(m => m)
                    .ToList();
                ViewData["search7"] = search7;
            }
            if (!string.IsNullOrEmpty(search8))
            {
                jous = jous
                    .Where(j => j.idobject.ToString().Contains(search8))
                    .Select(j => j)
                    .ToList();
                ViewData["search8"] = search8;
            }
            if (!string.IsNullOrEmpty(search9))
            {
                jous = jous
                    .Where(j => j.description.Contains(search9))
                    .Select(j => j)
                    .ToList();
                ViewData["search9"] = search9;
            }

            ViewData["NameSort"] = sort == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["UserTelSort"] = sort == SortState.UserTelAsc ? SortState.UserTelDesc : SortState.UserTelAsc;
            ViewData["UserEmailSort"] = sort == SortState.UserEmailAsc ? SortState.UserEmailDesc : SortState.UserEmailAsc;
            ViewData["OrgNameSort"] = sort == SortState.OrgNameAsc ? SortState.OrgNameDesc : SortState.OrgNameAsc;
            ViewData["UserRoleSort"] = sort == SortState.UserRoleAsc ? SortState.UserRoleDesc : SortState.UserRoleAsc;
            ViewData["OrgTelSort"] = sort == SortState.OrgTelAsc ? SortState.OrgTelDesc : SortState.OrgTelAsc;
            ViewData["OrgEmailSort"] = sort == SortState.OrgEmailAsc ? SortState.OrgEmailDesc : SortState.OrgEmailAsc;
            ViewData["DateSort"] = sort == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;
            ViewData["NumberSort"] = sort == SortState.NumberAsc ? SortState.NumberDesc : SortState.NumberAsc;
            ViewData["DescSort"] = sort == SortState.DescAsc ? SortState.DescDesc : SortState.DescAsc;
            jous = sort switch
            {
                SortState.NameAsc => jous.OrderBy(j => j.Usercapture.surname + " " + j.Usercapture.name + " " + j.Usercapture.patronymic),
                SortState.NameDesc => jous.OrderByDescending(j => j.Usercapture.surname + " " + j.Usercapture.name + " " + j.Usercapture.patronymic),
                SortState.UserTelAsc => jous.OrderBy(j => j.Usercapture.telephone),
                SortState.UserTelDesc => jous.OrderByDescending(j => j.Usercapture.telephone),
                SortState.UserEmailAsc => jous.OrderBy(j => j.Usercapture.email),
                SortState.UserEmailDesc => jous.OrderByDescending(j => j.Usercapture.email),
                SortState.OrgNameAsc => jous.OrderBy(j => j.Usercapture.Organization.name),
                SortState.OrgNameDesc => jous.OrderByDescending(j => j.Usercapture.Organization.name),
                SortState.UserRoleAsc => jous.OrderBy(j => j.Usercapture.Role.name),
                SortState.UserRoleDesc => jous.OrderByDescending(j => j.Usercapture.Role.name),
                SortState.OrgTelAsc => jous.OrderBy(j => j.Usercapture.Organization.telephone),
                SortState.OrgTelDesc => jous.OrderByDescending(j => j.Usercapture.Organization.telephone),
                SortState.OrgEmailAsc => jous.OrderBy(j => j.Usercapture.Organization.email),
                SortState.OrgEmailDesc => jous.OrderByDescending(j => j.Usercapture.Organization.email),
                SortState.DateAsc => jous.OrderBy(j => j.datetimechange),
                SortState.DateDesc => jous.OrderByDescending(j => j.datetimechange),
                SortState.NumberAsc => jous.OrderBy(j => j.idobject),
                SortState.NumberDesc => jous.OrderByDescending(j => j.idobject),
                SortState.DescAsc => jous.OrderBy(j => j.description),
                SortState.DescDesc => jous.OrderByDescending(j => j.description)
            };

            ViewData["tablenum"] = id;
            ViewData["config"] = _configuration;
            return View(jous);
        }

        [HttpGet]
        [Route("/journal/export/{id}")]
        public async Task<FileStreamResult> ExportJournal(int id)
        {
            return await _service.GetExcelJournal(id);
        }
    }
}
