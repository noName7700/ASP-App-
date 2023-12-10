using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;
using System.Linq;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Animal")]
    public class AnimalController : Controller
    {
        ApplicationContext _context;

        public AnimalController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Animal>> Get()
        {
            return await _context.animal.ToListAsync();
        }

        [HttpGet]
        [Route("/api/Animal/{id}")]
        public async Task<IEnumerable<Animal>> Get(int id)
        {
            return await _context.animal
                .Include(a => a.ActCapture)
                .ThenInclude(a => a.Locality)
                .Where(a => a.actcaptureid == id)
                .Select(a => a)
                .ToListAsync();
        }

        [HttpGet]
        [Route("/api/Animal/one/{id}")]
        public async Task<Animal> GetOne(int id)
        {
            return await _context.animal
                .Include(a => a.ActCapture)
                .ThenInclude(a => a.Locality)
                .Where(l => l.id == id)
                .FirstAsync();
        }

        [HttpGet]
        [Route("/api/Animal/last")]
        public async Task<Animal> GetLast()
        {
            return await _context.animal
                .Include(a => a.ActCapture)
                .ThenInclude(a => a.Locality)
                .Select(t => t)
                .OrderBy(t => t.id)
                .LastAsync();
        }

        [HttpPost]
        [Route("/api/Animal/add")]
        public async Task Post([FromBody] Animal value)
        {
            await _context.animal.AddAsync(value);
            await _context.SaveChangesAsync();
        }

        [HttpPut]
        [Route("/api/Animal/put/{id}")]
        public async Task Put(int id, [FromBody] Animal value)
        {
            var currentAnimal = await _context.animal.FirstOrDefaultAsync(a => a.id == id);
            if (currentAnimal != null)
            {
                currentAnimal.sex = value.sex;
                currentAnimal.breed = value.breed;
                currentAnimal.ears = value.ears;
                currentAnimal.wool = value.wool;
                currentAnimal.category = value.category;
                currentAnimal.color = value.color;
                currentAnimal.size = value.size;
                currentAnimal.tail = value.tail;
                currentAnimal.specsings = value.specsings;
                await _context.SaveChangesAsync();
            }
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Выбраны неверные данные.");
            }
        }

        // удалить животное
        [HttpDelete]
        [Route("/api/Animal/delete/{id}")]
        public async Task Delete(int id)
        {
            var currentAnimal = await _context.animal.FirstOrDefaultAsync(a => a.id == id);
            if (currentAnimal != null)
            {
                _context.animal.Remove(currentAnimal);
                await _context.SaveChangesAsync();
            }
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Не выбрано животное.");
            }
        }
    }
}
