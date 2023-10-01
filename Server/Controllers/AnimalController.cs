using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;

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

        //вывести одно животное по id
        [HttpGet("{id}")]
        public async Task<Animal> Get(int id)
        {
            return await _context.animal.FirstOrDefaultAsync(a => a.id == id);
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
        [HttpPut("{id}")]
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
        [HttpDelete("{id}")]
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
