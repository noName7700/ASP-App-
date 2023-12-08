using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;
using System.Text.RegularExpressions;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/User")]
    public class UserController : Controller
    {
        ApplicationContext _context;

        public UserController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Usercapture>> Get()
        {
            return await _context.usercapture
                .Include(u => u.Municipality)
                .Include(u => u.Locality)
                .Include(u => u.Organization)
                .Include(u => u.Role)
                .ToListAsync();
        }

        [HttpGet]
        [Route("/api/User/last")]
        public async Task<Usercapture> GetLast()
        {
            return await _context.usercapture
                .Include(u => u.Municipality)
                .Include(u => u.Locality)
                .Include(u => u.Organization)
                .Include(u => u.Role)
                .Select(t => t)
                .OrderBy(t => t.id)
                .LastAsync();
        }

        [HttpGet]
        [Route("/api/User/one/{id}")]
        public async Task<Usercapture> GetOne(int id)
        {
            return await _context.usercapture
                .Include(u => u.Municipality)
                .Include(u => u.Locality)
                .Include(u => u.Organization)
                .Include(u => u.Role)
                .Where(u => u.id == id)
                .FirstOrDefaultAsync();
        }

        [HttpPost]
        [Route("/api/User/add")]
        public async Task Post([FromBody] Usercapture value)
        {
            if (!Regex.IsMatch(value.surname, @"\p{IsCyrillic}"))
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Фамилия должна состоять только из букв.");
            }
            else if (!Regex.IsMatch(value.name, @"\p{IsCyrillic}"))
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Имя должно состоять только из букв.");
            }
            else if (!Regex.IsMatch(value.patronymic, @"\p{IsCyrillic}"))
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Отчество должно состоять только из букв.");
            }
            else if (value.telephone.Length != 11)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Номер телефона должен состоять только из 11 цифр.");
            }
            else
            {
                await _context.usercapture.AddAsync(value);
                await _context.SaveChangesAsync();
            }
        }

        [HttpPut]
        [Route("/api/User/put/{id}")]
        public async Task Put(int id, [FromBody] Usercapture value)
        {
            var currentUser = await _context.usercapture.FirstOrDefaultAsync(t => t.id == id);
            if (currentUser == null)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Данные пользователя введены неверно");
            }
            if (!Regex.IsMatch(value.surname, @"\p{IsCyrillic}"))
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Фамилия должна состоять только из букв.");
            }
            else if (!Regex.IsMatch(value.name, @"\p{IsCyrillic}"))
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Имя должно состоять только из букв.");
            }
            else if (!Regex.IsMatch(value.patronymic, @"\p{IsCyrillic}"))
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Отчество должно состоять только из букв.");
            }
            else if (value.telephone.Length != 11)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Номер телефона должен состоять только из 11 цифр.");
            }
            else
            {
                currentUser.surname = value.surname;
                currentUser.name = value.name;
                currentUser.patronymic = value.patronymic;
                currentUser.roleid = value.roleid;
                currentUser.municipalityid = value.municipalityid;
                currentUser.localityid = value.localityid;
                currentUser.organizationid = value.organizationid;
                currentUser.telephone = value.telephone;
                currentUser.email = value.email;
                await _context.SaveChangesAsync();
            }
        }
    }
}
