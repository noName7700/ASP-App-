using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/ActCapture")]
    public class ActCaptureController : Controller
    {
        ApplicationContext _context;
        public ActCaptureController(ApplicationContext context)
        {
            _context = context;
        }

        // вывести все акты отлова 
        [HttpGet]
        public async Task<IEnumerable<ActCapture>> Get()
        {
            return await _context.actcapture
                .Include(act => act.Animal)
                .Include(act => act.Locality)
                .ToListAsync();
        }

        // вывести акты отлова в одном нас пункте
        [HttpGet("{locid}")]
        public async Task<IEnumerable<ActCapture>> Get(int locid)
        {
            return await _context.actcapture
                .Include(act => act.Animal)
                .Include(act => act.Locality)
                .Select(ac => ac)
                .Where(ac => ac.localityid == locid)
                .ToListAsync();
        }

        // тут вывести акты отлова с сортировкой по дате
        [HttpGet("{datestart}/{dateend}/{locid}")]
        public async Task<IEnumerable<ActCapture>> Get(DateTime datestart, DateTime dateend, int locid)
        {
            return await _context.actcapture
                .Include(act => act.Animal)
                .Include(act => act.Locality)
                .Select(ac => ac)
                .Where(ac => ac.datecapture.Kind >= datestart.Kind && ac.datecapture.Kind <= dateend.Kind && ac.localityid == locid)
                .ToListAsync();
        }

        // добавить акт отлова (т.е. одну запись с одним животным)
        [HttpPost]
        public async Task Post([FromBody] ActCapture value)
        {
            await _context.actcapture.AddAsync(value);
            await _context.SaveChangesAsync();
        }

        // удалить животное 
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var currentAct = await _context.actcapture.FirstOrDefaultAsync(s => s.animalid == id);
            if (currentAct != null)
            {
                _context.actcapture.Remove(currentAct);
                await _context.SaveChangesAsync();
            }
        }

    }
}
