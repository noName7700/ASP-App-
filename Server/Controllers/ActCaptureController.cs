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
        public IEnumerable<ActCapture> Get()
        {
            return _context.actcapture
                .Include(act => act.Animal)
                .Include(act => act.Locality)
                .ToList();
        }

        // вывести акты отлова в одном нас пункте
        [HttpGet("{locid}")]
        public IEnumerable<ActCapture> Get(int locid)
        {
            return _context.actcapture
                .Include(act => act.Animal)
                .Include(act => act.Locality)
                .Select(ac => ac)
                .Where(ac => ac.localityid == locid)
                .ToList();
        }

        // тут вывести акты отлова с сортировкой по дате
        [HttpGet("{datestart}/{dateend}/{locid}")]
        public IEnumerable<ActCapture> Get(DateTime datestart, DateTime dateend, int locid)
        {
            return _context.actcapture
                .Include(act => act.Animal)
                .Include(act => act.Locality)
                .Select(ac => ac)
                .Where(ac => ac.datecapture.Kind >= datestart.Kind && ac.datecapture.Kind <= dateend.Kind && ac.localityid == locid)
                .ToList();
        }

        // добавить акт отлова (т.е. одну запись с одним животным)
        [HttpPost]
        public void Post([FromBody] ActCapture value)
        {
            _context.actcapture.Add(value);
            _context.SaveChanges();
        }

        // удалить животное 
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var currentAct = _context.actcapture.FirstOrDefault(s => s.animalid == id);
            if (currentAct != null)
            {
                _context.actcapture.Remove(currentAct);
                _context.SaveChanges();
            }
        }

    }
}
