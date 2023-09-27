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
        public async Task<IEnumerable<Contract>> Get()
        {
            return await _context.contract
                .Include(c => c.ActCapture)
                .Include(c => c.Schedule)
                .ToListAsync();
        }

        // создать контракт (т.е. одну запись)
        [HttpPost]
        public async Task Post([FromBody] Contract value)
        {
            await _context.contract.AddAsync(value);
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
