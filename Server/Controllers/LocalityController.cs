using Domain;
using Microsoft.AspNetCore.Mvc;
using Server.Application;

namespace Server.Controllers
{
    [ApiController]
    [Route("Locality")]
    public class LocalityController : Controller
    {
        ApplicationContext _context;

        public LocalityController(ApplicationContext context)
        {
            _context = context;
        }

        // вывести все населенные пункты
        [HttpGet]
        public IEnumerable<Locality> Get()
        {
            return _context.locality.ToList();
        }

        // вывести один нас пункт
        [HttpGet("{id}")]
        public Locality Get(int id)
        {
            return _context.locality.FirstOrDefault(l => l.id == id);
        }

        // добавить новый нас пункт
        [HttpPost]
        public void Post([FromBody] Locality value)
        {
            _context.locality.Add(value);
            _context.SaveChanges();
        }

        // изменить нас пункт
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Locality value)
        {
            var currentLoc = _context.locality.FirstOrDefault(l => l.id == id);
            if (currentLoc != null)
            {
                currentLoc.name = value.name;
                currentLoc.tariph = value.tariph;
                _context.SaveChanges();
            }
        }

        // удалить нас пункт
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var currentLoc = _context.locality.FirstOrDefault(l => l.id == id);
            if (currentLoc != null)
            {
                _context.locality.Remove(currentLoc);
                _context.SaveChanges();
            }
        }
    }
}
