using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;
using System.Text.RegularExpressions;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Locality")]
    public class LocalityController : Controller
    {
        ApplicationContext _context;

        public LocalityController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Locality>> Get()
        {
            return await _context.locality.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<Locality>> Get(int id)
        {
            var t = await _context.locality
                .Where(l => l.municipalityid == id)
                .Select(l => l)
                .ToListAsync();
            return t;
        }

        [HttpGet]
        [Route("/api/Locality/one/{id}")]
        public async Task<Locality> GetOne(int id)
        {
            return await _context.locality
                .Include(l => l.Municipality)
                .Where(l => l.id == id)
                .FirstAsync();
        }

        [HttpGet]
        [Route("/api/Locality/last")]
        public async Task<Locality> GetLast()
        {
            return await _context.locality.Select(t => t).OrderBy(t => t.id).LastAsync();
        }

        [HttpPost]
        [Route("/api/Locality/add")]
        public async Task Post([FromBody] Locality value)
        {
            var countLoc = await _context.locality
                .Where(l => l.municipalityid == value.municipalityid && l.name == value.name)
                .CountAsync();

            if (!Regex.IsMatch(value.name, @"^[а-яА-Я]+$"))
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Название населенного пункта должно содержать только буквы.");
            }
            else if (countLoc == 0)
            {
                await _context.locality.AddAsync(value);
                await _context.SaveChangesAsync();
            }
            else if (countLoc != 0)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Населенный пункт с таким названием уже существует.");
            }
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Введены неверные данные.");
            }
        }

        [HttpPut]
        [Route("/api/Locality/put/{id}")]
        public async Task Put(int id, [FromBody] Locality value)
        {
            var countLoc = await _context.locality
                .Where(l => l.municipalityid == value.municipalityid && l.name == value.name)
                .CountAsync();

            var currentLoc = await _context.locality.FirstOrDefaultAsync(l => l.id == id);
            if (currentLoc != null && !Regex.IsMatch(value.name, @"^[а-яА-Я]+$"))
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Название населенного пункта должно содержать только буквы.");
            }
            else if (currentLoc != null && countLoc == 0)
            {
                currentLoc.name = value.name;
                await _context.SaveChangesAsync();
            }
            else if (currentLoc != null && countLoc != 0)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Населенный пункт с таким названием уже существует.");
            }
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Введены неверные данные.");
            }
        }

        [HttpDelete]
        [Route("/api/Locality/delete/{id}")]
        public async Task Delete(int id)
        {
            var currentLoc = await _context.locality.FirstOrDefaultAsync(l => l.id == id);
            if (currentLoc != null)
            {
                _context.locality.Remove(currentLoc);
                await _context.SaveChangesAsync();
            }
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Населенный пункт не выбран.");
            }
        }
    }
}
