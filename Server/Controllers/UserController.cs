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
    }
}
