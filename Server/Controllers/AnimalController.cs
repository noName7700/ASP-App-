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

        // вывести всех животных
        [HttpGet]
        public async Task<IEnumerable<Animal>> Get()
        {
            return await _context.animal.ToListAsync();
        }

        //вывести одно животное по localid и datecapture
        [HttpGet]
        [Route("/api/Animal/{id}/{date}")]
        public async Task<IEnumerable<Animal>> Get(int id, string date)
        {
            var d = DateTime.Parse(date);
            var res = await _context.actcapture
                .Include(a => a.Animal)
                .Where(a => a.idlocality == id && a.datecapture.Year == d.Year 
                && a.datecapture.Month == d.Month && a.datecapture.Day == d.Day)
                .Select(a => a.Animal)
                .ToListAsync();
            return res;
        }

        [HttpGet]
        [Route("/api/Animal/one/{id}")]
        public async Task<Animal> GetOne(int id)
        {
            return await _context.animal.Where(l => l.id == id).FirstAsync();
        }

        [HttpGet]
        [Route("/api/Animal/last")]
        public async Task<Animal> GetLast()
        {
            return await _context.animal.Select(t => t).OrderBy(t => t.id).LastAsync();
        }

        // добавить новое животное
        [HttpPost]
        [Route("/api/Animal/add")]
        public async Task Post([FromBody] Animal value)
        {
            await _context.animal.AddAsync(value);
            await _context.SaveChangesAsync();
        }

        // изменить животное
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
                currentAnimal.specsigns = value.specsigns;
                await _context.SaveChangesAsync();
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
        }
    }
}
