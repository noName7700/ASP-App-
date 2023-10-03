using Microsoft.AspNetCore.Mvc;
using Server.Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Contract")]
    public class ContractController : Controller
    {
        ApplicationContext _context;

        public ContractController(ApplicationContext context)
        {
            _context = context;
        }

        // получить все контракты
        [HttpGet]
        public async Task<IEnumerable<ContractNumber>> Get()
        {
            return await _context.contractnumber
                .ToListAsync();
        }

        // создать контракт (т.е. одну запись)
        [HttpPost]
        [Route("/api/Contract/add")]
        public async Task Post([FromBody] ContractNumber value)
        {
            await _context.contractnumber.AddAsync(value);
            await _context.SaveChangesAsync();
        }

        // удалить контракт по id
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var currentCon = await _context.contract.FirstOrDefaultAsync(c => c.id == id);
            if (currentCon != null)
            {
                _context.contract.Remove(currentCon);
                await _context.SaveChangesAsync();
            }
        }
    }
}
