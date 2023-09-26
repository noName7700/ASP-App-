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
        public IEnumerable<Animal> Get()
        {
            return _context.animal.ToList();
        }

        //вывести одно животное по id
        [HttpGet("{id}")]
        public Animal Get(int id)
        {
            return _context.animal.FirstOrDefault(a => a.id == id);
        }

        // добавить новое животное
        [HttpPost]
        public void Post([FromBody] Animal value)
        {
            _context.animal.Add(value);
            _context.SaveChanges();
        }

        // изменить животное
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Animal value)
        {
            var currentAnimal = _context.animal.FirstOrDefault(a => a.id == id);
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
                _context.SaveChanges();
            }
        }

        // удалить животное
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var currentAnimal = _context.animal.FirstOrDefault(a => a.id == id);
            if (currentAnimal != null)
            {
                _context.animal.Remove(currentAnimal);
                _context.SaveChanges();
            }
        }
    }
}
