using Microsoft.AspNetCore.Mvc;
using Server.Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Municipality")]
    public class MunicipalityController
    {
        ApplicationContext _context;

        public MunicipalityController(ApplicationContext context)
        {
            _context = context;
        }

        // все муниципалитеты
        [HttpGet]
        public async Task<IEnumerable<Municipality>> Get()
        {
            return await _context.municipality
                .Include(m => m.Locality)
                .Include(m => m.Contract)
                .ToListAsync();
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
