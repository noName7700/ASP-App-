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
        public async Task<double> Get(DateTime stardDate, DateTime endDate, string namemun)
        {
            // выбираю нужные населенные пункты
            var needLocalities = await _context.municipality.Select(aa => aa).Where(aa => aa.name == namemun).Select(h => h.localityid).ToListAsync();

            // то что фактически получилось, я пробегаюсь по всем актам отлова из этого нас пункта и считаю цену потом складываю
            return await _context.actcapture
                .Include(act => act.Locality)
                .Where(act => act.datecapture >= stardDate && act.datecapture <= endDate && needLocalities.Contains(act.localityid))
                .SumAsync(act => act.Locality.tariph);
        }

        [HttpGet("{namemun}")]
        public async Task<Dictionary<int, int>> Get(string namemun)
        {
            // выбираю нужные населенные пункты
            var needLocalities = await _context.municipality.Select(aa => aa).Where(aa => aa.name == namemun).Select(h => h.localityid).ToListAsync();

            //считаю по планам-графикам то что запланировано
            var countPlanAnimal = await _context.schedule
                .Include(sch => sch.TaskMonth)
                .Where(sch => needLocalities.Contains(sch.localityid))
                .SumAsync(sch => sch.TaskMonth.countanimal);

            // считаю по актам отлова то что в итоге
            var countAnimal = await _context.actcapture
                .Where(act => needLocalities.Contains(act.localityid))
                .CountAsync();

            return new Dictionary<int, int> { { countPlanAnimal, countAnimal } };
        }
    }
}
