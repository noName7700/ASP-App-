using Microsoft.AspNetCore.Mvc;
using Server.Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers
{
    [ApiController]
    [Route("Municipality")]
    public class MunicipalityController
    {
        ApplicationContext _context;

        public MunicipalityController(ApplicationContext context)
        {
            _context = context;
        }

        // все муниципалитеты
        [HttpGet]
        public IEnumerable<Municipality> Get()
        {
            return _context.municipality
                .Include(m => m.Locality)
                .Include(m => m.Contract)
                .ToList();
        }

        // создать новый муниципалитет
        [HttpPost]
        public void Post([FromBody] Municipality value)
        {
            _context.municipality.Add(value);
            _context.SaveChanges();
        }
    }
}
