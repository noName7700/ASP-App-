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

        [HttpGet]
        public async Task<IEnumerable<Contract_Locality>> Get()
        {
            return await _context.contract_locality
                .Include(cl => cl.Contract)
                .Include(cl => cl.Locality)
                .Select(cl => cl)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<Contract_Locality>> Get(int id)
        {
            var t = await _context.contract_locality
                .Include(cl => cl.Locality)
                .Include(cl => cl.Contract)
                .Where(cl => cl.contractid == id)
                .Select(cl => cl)
                .ToListAsync();
            return t;
        }

        [HttpGet]
        [Route("/api/Contract_Locality/one/{id}")]
        public async Task<Contract_Locality> GetOne(int id)
        {
            var t = await _context.contract_locality
                .Include(cl => cl.Contract)
                .Include(cl => cl.Locality)
                .Where(cl => cl.localityid == id)
                .Select(cl => cl)
                .FirstOrDefaultAsync();
            return t;
        }

        [HttpGet]
        [Route("/api/Contract_Locality/date/{id}/{datecapture}")]
        public async Task<Contract_Locality> GetDate(int id, string datecapture)
        {
            DateTime date = DateTime.Parse(datecapture);

            foreach (var cl in await _context.contract_locality.Include(cl => cl.Contract).ToListAsync())
            {
                if (cl.localityid == id && cl.Contract.dateconclusion <= date && cl.Contract.validityperiod >= date)
                    return cl;
            }
            return null;
            //var t = await _context.contract_locality
            //    .Include(cl => cl.Contract)
            //    .Include(cl => cl.Locality)
            //    .Where(cl => cl.localityid == id && cl.Contract.dateconclusion <= date && cl.Contract.validityperiod >= date)
            //    .Select(cl => cl)
            //    .FirstOrDefaultAsync();
            //return t;
        }

        // изменить нас пункт
        [HttpPut]
        [Route("/api/Contract_Locality/put/{id}")]
        public async Task Put(int id, [FromBody] Contract_Locality value)
        {
            var contractValue = await _context.contract_locality
                .Include(cl => cl.Contract)
                .Where(cl => cl.id == id)
                .Select(cl => cl.Contract)
                .FirstOrDefaultAsync();

            var countContract = await _context.contract
                .Where(c => c.municipalityid == contractValue.municipalityid && c.validityperiod >= contractValue.dateconclusion)
                .CountAsync();

            var currentConLoc = await _context.contract_locality.FirstOrDefaultAsync(l => l.id == id);
            if (currentConLoc != null || countContract == 0)
            {
                currentConLoc.tariph = value.tariph;
                await _context.SaveChangesAsync();
            }
            //else
            //{
            //    тут выводится ошибка
            //}
        }

        // добавить цену за животное в нас пункте по контракту
        [HttpPost]
        [Route("/api/Contract_Locality/add")]
        public async Task Post([FromBody] Contract_Locality value)
        {
            var contractValue = await _context.contract
                .Where(c => c.id == value.contractid)
                .Select(c => c)
                .FirstOrDefaultAsync();

            var countContract = await _context.contract
                .Where(c => c.municipalityid == contractValue.municipalityid && c.validityperiod >= contractValue.dateconclusion)
                .CountAsync();

            if (countContract == 0)
            {
                await _context.contract_locality.AddAsync(value);
                await _context.SaveChangesAsync();
            }
            //else
            //{
            //    тут выводится ошибка
            //}
        }
    }
}
