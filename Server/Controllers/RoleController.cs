using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;
using System.Text.RegularExpressions;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Role")]
    public class RoleController : Controller
    {
        ApplicationContext _context;

        public RoleController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Role>> Get()
        {
            return await _context.role
                .ToListAsync();
        }

        [HttpGet]
        [Route("/api/Role/{id}")]
        public async Task<Role> Get(int id)
        {
            return await _context.role
                .Where(r => r.id == id)
                .Select(r => r)
                .FirstOrDefaultAsync();
        }

        [HttpGet]
        [Route("/api/Role/last")]
        public async Task<Role> GetLast()
        {
            return await _context
                .role
                .Select(t => t)
                .OrderBy(t => t.id)
                .LastAsync();
        }

        // добавить новую роль
        [HttpPost]
        [Route("/api/Role/add")]
        public async Task Post([FromBody] Role value)
        {
            if (!Regex.IsMatch(value.name, @"^[а-яА-Я]+$"))
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Название роли должно состоять только из букв.");
            }
            else
            {
                await _context.role.AddAsync(value);
                await _context.SaveChangesAsync();
            }
        }

        [HttpPut]
        [Route("/api/Role/put/{id}")]
        public async Task Put(int id, [FromBody] Role value)
        {
            var currentRole = await _context.role.FirstOrDefaultAsync(t => t.id == id);
            if (currentRole != null && Regex.IsMatch(value.name, @"^[а-яА-Я]+$"))
            {
                currentRole.name = value.name;
                await _context.SaveChangesAsync();
            }
            else if (currentRole != null)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Название роли должно состоять только из букв.");
            }
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Введены неверные данные.");
            }
        }

        [HttpDelete]
        [Route("/api/Role/delete/{id}")]
        public async Task Delete(int id)
        {
            var currentRole = await _context.role.FirstOrDefaultAsync(c => c.id == id);
            if (currentRole != null)
            {
                _context.role.Remove(currentRole);
                await _context.SaveChangesAsync();
            }
        }
    }
}
