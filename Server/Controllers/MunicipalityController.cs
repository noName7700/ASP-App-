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
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<Locality>> Get(int id)
        {
            return await _context.municipality
                .Include(m => m.Locality)
                .Include(m => m.Contract)
                .Select(m => m.Locality)
                .ToListAsync();
        }

        // создать новый муниципалитет
        [HttpPost]
        public async Task Post([FromBody] Municipality value)
        {
            await _context.municipality.AddAsync(value);
            await _context.SaveChangesAsync();
        }
    }
}
