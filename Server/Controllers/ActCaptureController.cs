using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;
using System.Linq;

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

        //ЭТО Я ДОБАВИЛА СЕЙЧАС
        // вывести все акты отлова (т.е. нас пункты:))
        [HttpGet]
        public async Task<IEnumerable<Locality>> Get()
        {
            // выводятся только наши нас пункты
            return await _context.locality
                .ToListAsync();
        }

        // ЭТО ИЗМЕНИЛА СЕЙЧАС
        // вывести акты отлова в одном нас пункте
        // т.е. группировка животных по дате типа (дата - животные в эту дату)
        [HttpGet("{locid}")]
        public async Task<IEnumerable<ActCapture>> Get(int locid)
        {
            return await _context.actcapture
                .Include(a => a.Animal)
                .Include(a => a.Locality)
                .Where(a => a.idlocality == locid)
                .GroupBy(a => a.datecapture)
                .OrderByDescending(a => a.Key)
                .Select(f => f.First())
                .ToListAsync();

            // это открываются акт отлова для одного нас пункта (выводиться будут только дата и кнопка просмотр животного)
            //return await _context.actcapture
            //    .Include(act => act.Animal)
            //    .Include(act => act.Locality)
            //    .Select(ac => ac)
            //    .Where(ac => ac.localityid == locid)
            //    .ToListAsync();
        }

        [HttpGet]
        [Route("/api/ActCapture/{locid}/{date}")]
        public async Task<IEnumerable<ActCapture>> GetActs(int locid, string date)
        {
            return await _context.actcapture
                .Where(a => a.idlocality == locid && 
                a.datecapture.Year == DateTime.Parse(date).Year
                && a.datecapture.Month == DateTime.Parse(date).Month
                && a.datecapture.Day == DateTime.Parse(date).Day)
                .Select(a => a)
                .ToListAsync();
        }

        [HttpGet]
        [Route("/api/ActCapture/animal/{id}")]
        public async Task<ActCapture> GetFromAnimalId(int id)
        {
            return await _context.actcapture.Where(a => a.idanimal == id).Select(a => a).FirstAsync();
        }

        // тут вывести акты отлова с сортировкой по дате
        /*[HttpGet("{datestart}/{dateend}/{locid}")]
        public async Task<IEnumerable<ActCapture>> Get(DateTime datestart, DateTime dateend, int locid)
        {
            return await _context.actcapture
                .Include(act => act.Animal)
                .Include(act => act.Locality)
                .Select(ac => ac)
                .Where(ac => ac.datecapture.Kind >= datestart.Kind && ac.datecapture.Kind <= dateend.Kind && ac.localityid == locid)
                .ToListAsync();
        }*/

        // добавить акт отлова (т.е. одну запись с одним животным)
        [HttpPost]
        [Route("/api/ActCapture/add")]
        public async Task Post([FromBody] ActCapture value)
        {
            await _context.actcapture.AddAsync(value);
            await _context.SaveChangesAsync();
        }

        [HttpPut]
        [Route("/api/ActCapture/put/{id}")]
        public async Task Put(int id, [FromBody] ActCapture value)
        {
            var currentLoc = await _context.actcapture.FirstOrDefaultAsync(l => l.id == id);
            if (currentLoc != null)
            {
                currentLoc.datecapture = value.datecapture;
                currentLoc.idlocality = value.idlocality;
                await _context.SaveChangesAsync();
            }
        }

        // удалить акт 
        [HttpDelete]
        [Route("/api/ActCapture/delete/{id}")]
        public async Task Delete(int id)
        {
            var currentAct = await _context.actcapture.FirstOrDefaultAsync(s => s.id == id);
            if (currentAct != null)
            {
                _context.actcapture.Remove(currentAct);
                await _context.SaveChangesAsync();
            }
        }

    }
}
