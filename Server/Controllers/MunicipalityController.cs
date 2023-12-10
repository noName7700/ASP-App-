using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;
using System.Text.RegularExpressions;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Municipality")]
    public class MunicipalityController : Controller
    {
        ApplicationContext _context;

        public MunicipalityController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Municipality>> Get()
        {
            return await _context.municipality
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<Municipality> Get(int id)
        {
            return await _context.municipality
                .Where(m => m.id == id)
                .Select (m => m)
                .FirstOrDefaultAsync();
        }

        [HttpGet]
        [Route("/api/Municipality/last")]
        public async Task<Municipality> GetLast()
        {
            return await _context.municipality
                .Select(t => t)
                .OrderBy(t => t.id)
                .LastAsync();
        }

        [HttpPost]
        [Route("/api/Municipality/add")]
        public async Task Post([FromBody] Municipality value)
        {
            var countMun = await _context.municipality
                .Where(m => m.name == value.name)
                .CountAsync();

            if (!Regex.IsMatch(value.name, @"^[а-яА-Я]+$"))
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Название муниципалитета должно содержать только буквы.");
            }
            else if (countMun == 0)
            {
                await _context.municipality.AddAsync(value);
                await _context.SaveChangesAsync();
            }
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Муниципалитет с таким названием уже существует.");
            }
        }

        [HttpGet]
        [Route("/api/Municipality/loc/{id}")]
        public async Task<Municipality> GetFromLocalityId(int id)
        {
            return await _context.locality
                .Include(l => l.Municipality)
                .Where(l => l.id == id)
                .Select(m => m.Municipality)
                .FirstAsync();
        }

        //[HttpPost]
        //[Route("/api/Municipality/add-loc")]
        //public async Task Post([FromBody] Municipality_Locality value)
        //{
        //    await _context.municipality_locality.AddAsync(value);
        //    await _context.SaveChangesAsync();
        //}

        //[HttpDelete]
        //[Route("/api/Municipality/delete/{id}")]
        //public async Task Delete(int id)
        //{
        //    var currentMun = await _context.municipality_locality.FirstOrDefaultAsync(s => s.id == id);
        //    if (currentMun != null)
        //    {
        //        _context.municipality_locality.Remove(currentMun);
        //        await _context.SaveChangesAsync();
        //    }
        //}
    }
}
