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
            return await _context.locality.Where(l => l.id == id).FirstAsync();
        }

        [HttpGet]
        [Route("/api/Locality/last")]
        public async Task<Locality> GetLast()
        {
            return await _context.locality.Select(t => t).OrderBy(t => t.id).LastAsync();
        }


        // добавить новый нас пункт
        [HttpPost]
        [Route("/api/Locality/add")]
        public async Task Post([FromBody] Locality value)
        {
            var countLoc = await _context.locality
                .Where(l => l.municipalityid == value.municipalityid && l.name == value.name)
                .CountAsync();

            if (countLoc == 0)
            {
                await _context.locality.AddAsync(value);
                await _context.SaveChangesAsync();
            }
            //else
            //{
            //    ошибка
            //}
        }

        // изменить нас пункт
        [HttpPut]
        [Route("/api/Locality/put/{id}")]
        public async Task Put(int id, [FromBody] Locality value)
        {
            var currentLoc = await _context.locality.FirstOrDefaultAsync(l => l.id == id);
            if (currentLoc != null)
            {
                currentLoc.name = value.name;
                await _context.SaveChangesAsync();
            }
        }

        // удалить нас пункт
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
        }
    }
}
