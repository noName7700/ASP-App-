using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;

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

        // вывести все населенные пункты
        [HttpGet]
        public async Task<IEnumerable<Locality>> Get()
        {
            return await _context.locality.ToListAsync();
        }

        //// вывести один нас пункт
        //[HttpGet("{id}")]
        //public async Task<Locality> Get(int id)
        //{
        //    return await _context.locality.FirstOrDefaultAsync(l => l.id == id);
        //}

        // вывести нас пункты одного муниципалитета
        [HttpGet("{id}")]
        public async Task<List<Locality>> Get(int id)
        {
            return await _context.municipality
                .Include(m => m.Locality)
                .Include(m => m.MunicipalityName)
                .Where(m => m.munid == id)
                .Select(m => m.Locality)
                .ToListAsync();
        }

        // добавить новый нас пункт
        [HttpPost]
        [Route("/api/Locality/add")]
        public async Task Post([FromBody] Locality value)
        {
            await _context.locality.AddAsync(value);
            await _context.SaveChangesAsync();
        }

        // изменить нас пункт
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] Locality value)
        {
            var currentLoc = await _context.locality.FirstOrDefaultAsync(l => l.id == id);
            if (currentLoc != null)
            {
                currentLoc.name = value.name;
                currentLoc.tariph = value.tariph;
                await _context.SaveChangesAsync();
            }
        }

        // удалить нас пункт
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var currentLoc = await _context.locality.FirstOrDefaultAsync(l => l.id == id);
            if (currentLoc != null)
            {
                _context.locality.Remove(currentLoc);
                await _context.SaveChangesAsync();
            }
        }
    }
}
