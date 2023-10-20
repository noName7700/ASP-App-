using Microsoft.AspNetCore.Mvc;
using Server.Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Contract_Locality")]
    public class Contract_LocalityController
    {
        ApplicationContext _context;

        public Contract_LocalityController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<Contract_Locality>> Get(int id)
        {
            // id контракта
            var conid = await _context.contract
                .Where(c => c.municipalityid == id)
                .Select(c => c.id)
                .FirstOrDefaultAsync();

            var t = await _context.contract_locality
                .Where(cl => cl.contractid == conid)
                .Select(cl => cl)
                .ToListAsync();
            return t;
        }

        [HttpGet]
        [Route("/api/Contract_Locality/one/{id}")]
        public async Task<Contract_Locality> GetOne(int id)
        {
            var t = await _context.contract_locality
                .Where(cl => cl.localityid == id)
                .Select(cl => cl)
                .FirstOrDefaultAsync();
            return t;
        }

        // изменить нас пункт
        [HttpPut]
        [Route("/api/Contract_Locality/put/{id}")]
        public async Task Put(int id, [FromBody] Contract_Locality value)
        {
            var currentConLoc = await _context.contract_locality.FirstOrDefaultAsync(l => l.id == id);
            if (currentConLoc != null)
            {
                currentConLoc.tariph = value.tariph;
                await _context.SaveChangesAsync();
            }
        }

        // добавить цену за животное в нас пункте по контракту
        [HttpPost]
        [Route("/api/Contract_Locality/add")]
        public async Task Post([FromBody] Contract_Locality value)
        {
            await _context.contract_locality.AddAsync(value);
            await _context.SaveChangesAsync();
        }
    }
}
