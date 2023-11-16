using Microsoft.AspNetCore.Mvc;
using Server.Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Journal")]
    public class JournalController : ControllerBase
    {
        ApplicationContext _context;

        public JournalController(ApplicationContext context)
        {
            _context = context;
        }

        // получить данные журнала для таблицы
        [HttpGet("{id}")]
        //[Route("/api/Journal/{id}")]
        public async Task<IEnumerable<Journal>> Get(int id)
        {
            var aa = await _context.journal
                .Include(j => j.Usercapture)
                .ThenInclude(j => j.Organization)
                .Where(j => j.nametable == id)
                .Select(j => j)
                .ToListAsync();
            return aa;
        }

        // добавить запись журнала
        [HttpPost]
        [Route("/api/Journal/add")]
        public async Task Post([FromBody] Journal value)
        {
            await _context.journal.AddAsync(value);
            await _context.SaveChangesAsync();
        }

        // удалить запись журнала
        [HttpDelete]
        [Route("/api/Journal/delete/{id}")]
        public async Task Delete(int id)
        {
            var currentJou = await _context.journal.FirstOrDefaultAsync(j => j.id == id);
            if (currentJou != null)
            {
                _context.journal.Remove(currentJou);
                await _context.SaveChangesAsync();
            }
        }
    }
}
