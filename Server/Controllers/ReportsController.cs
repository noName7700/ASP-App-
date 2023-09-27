using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Reports")]
    public class ReportsController : Controller
    {
        ApplicationContext _context;

        public ReportsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("{startDate}/{endDate}/{namemun}")]
        public double Get(DateTime stardDate, DateTime endDate, string namemun)
        {
            // выбираю нужные населенные пункты
            var needLocalities = _context.municipality.Select(aa => aa).Where(aa => aa.name == namemun).Select(h => h.localityid).ToList();

            // то что фактически получилось, я пробегаюсь по всем актам отлова из этого нас пункта и считаю цену потом складываю
            var totalSum = _context.actcapture
                .Include(act => act.Locality)
                .Where(act => act.datecapture >= stardDate && act.datecapture <= endDate && needLocalities.Contains(act.localityid))
                .Sum(act => act.Locality.tariph);
            return totalSum;
        }

        [HttpGet("{namemun}")]
        public Dictionary<int, int> Get(string namemun)
        {
            // выбираю нужные населенные пункты
            var needLocalities = _context.municipality.Select(aa => aa).Where(aa => aa.name == namemun).Select(h => h.localityid).ToList();

            //считаю по планам-графикам то что запланировано
            var countPlanAnimal = _context.schedule
                .Include(sch => sch.TaskMonth)
                .Where(sch => needLocalities.Contains(sch.localityid))
                .Sum(sch => sch.TaskMonth.countanimal);

            // считаю по актам отлова то что в итоге
            var countAnimal = _context.actcapture
                .Where(act => needLocalities.Contains(act.localityid))
                .Count();

            return new Dictionary<int, int> { { countPlanAnimal, countAnimal } };
        }
    }
}
