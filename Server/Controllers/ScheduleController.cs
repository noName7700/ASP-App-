using Microsoft.AspNetCore.Mvc;
using Server.Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Schedule")]
    public class ScheduleController : ControllerBase
    {
        ApplicationContext _context;

        public ScheduleController(ApplicationContext context)
        {
            _context = context;
        }

        // получить все планы-графики
        [HttpGet]
        public async Task<IEnumerable<Schedule>> Get()
        {
            return await _context.schedule
                .Include(sch => sch.Locality)
                .GroupBy(sch => new { sch.Locality.name, sch.dateapproval })
                .OrderByDescending(sch => sch.Key.name)
                .Select(f => f.First())
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<TaskMonth>> Get(int id)
        {
            return await _context.taskmonth
                .Include(t => t.Schedule)
                .Where(t => t.Schedule.localityid == id)
                .Select(t => t)
                .ToListAsync();
        }

        [HttpGet]
        [Route("/api/Schedule/last/{locid}")]
        public async Task<Schedule> GetLast(int locid)
        {
            return await _context.schedule
                .Where(s => s.localityid == locid)
                .Select(s => s)
                .OrderBy(s => s.id)
                .LastAsync();
        }

        [HttpGet]
        [Route("/api/Schedule/one/{locid}/{startdate}")]
        public async Task<Schedule> GetOne(int locid, string startdate)
        {
            var startDate = DateTime.Parse(startdate);
            var schedule = new Schedule();

            foreach(var sc in await _context.schedule.Include(sc => sc.Contract).ToListAsync())
                if (sc.localityid == locid && sc.Contract.dateconclusion <= startDate && sc.Contract.validityperiod >= startDate)
                    schedule = sc;

            return schedule;
        }


        [HttpGet]
        [Route("/api/Schedule/task/{id}")]
        public async Task<Schedule> GetFromTaskMonthId(int id)
        {
            return await _context.taskmonth
                .Include(t => t.Schedule)
                .ThenInclude(sc => sc.Locality)
                .Where(t => t.id == id)
                .Select(t => t.Schedule)
                .FirstAsync();
        }

        [HttpPost]
        [Route("/api/Schedule/add")]
        public async Task Post([FromBody] Schedule value)
        {
            var loc = await _context.locality
                .Where(l => l.id == value.localityid)
                .Select(l => l)
                .FirstOrDefaultAsync();

            var con = await _context.contract_locality
                .Include(cl => cl.Contract)
                .Where(cl => cl.localityid == loc.id && cl.Contract.dateconclusion <= value.dateapproval && cl.Contract.validityperiod >= value.dateapproval)
                .Select(cl => cl.Contract)
                .FirstOrDefaultAsync();

            var countSchedule = await _context.schedule
                .Where(sc => sc.contractid == con.id)
                .CountAsync();

            if (countSchedule == 0)
            {
                await _context.schedule.AddAsync(value);
                await _context.SaveChangesAsync();
            }
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync("Для данного населенного пункта по контракту план-график уже составлен");
            }
        }

        [HttpDelete]
        [Route("/api/Schedule/delete/{id}")]
        public async Task Delete(int id)
        {
            var currentSch = await _context.schedule.FirstOrDefaultAsync(s => s.id == id);
            if (currentSch != null)
            {
                _context.schedule.Remove(currentSch);
                await _context.SaveChangesAsync();
            }
        }
    }
}
