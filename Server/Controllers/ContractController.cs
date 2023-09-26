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
        public IEnumerable<Contract> Get()
        {
            return _context.contract
                .Include(c => c.ActCapture)
                .Include(c => c.Schedule)
                .ToList();
        }

        // создать контракт (т.е. одну запись)
        [HttpPost]
        public void Post([FromBody] Contract value)
        {
            _context.contract.Add(value);
            _context.SaveChanges();
        }

        // удалить контракт по id
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var currentCon = _context.contract.FirstOrDefault(c => c.id == id);
            if (currentCon != null)
            {
                _context.contract.Remove(currentCon);
                _context.SaveChanges();
            }
        }
    }
}
