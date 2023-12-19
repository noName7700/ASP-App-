using Microsoft.AspNetCore.Mvc;
using Server.Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Domain.NonDomain;
using Domain.ApplicationClasses;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Schedule")]
    public class ScheduleController : ControllerBase, IRegister<Schedule>
    {
        ApplicationContext _context;
        private readonly IRegister<Schedule> proxy;
        public ScheduleController(ApplicationContext context)
        {
            _context = context;
            proxy = new ScheduleFilterProxy(this);
        }

        public async Task<List<Schedule>> GetAll(Usercapture user, int id = 1)
        {
            return await _context.schedule
                .Include(s => s.Contract_Locality)
                .ThenInclude(cl => cl.Locality)
                .Include(s => s.Contract_Locality)
                .ThenInclude(cl => cl.Contract)
                .GroupBy(sch => new { sch.Contract_Locality.Locality.name, sch.dateapproval })
                .OrderByDescending(sch => sch.Key.name)
                .Select(f => f.First())
                .ToListAsync();
        }

        // получить все планы-графики
        [HttpGet]
        [Route("/api/Schedule/user/{userid}")]
        public async Task<IEnumerable<Schedule>> GetAll(int userid)
        {
            var user = await _context.usercapture
                .Where(u => u.id == userid)
                .FirstOrDefaultAsync();
            return await proxy.GetAll(user);
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<TaskMonth>> Get(int id)
        {
            var tt = await _context.taskmonth
                .Include(t => t.Schedule)
                .ThenInclude(s => s.Contract_Locality)
                .ThenInclude(cl => cl.Locality)
                .Include(t => t.Schedule)
                .ThenInclude(s => s.Contract_Locality)
                .ThenInclude(cl => cl.Contract)
                .Where(t => t.Schedule.Contract_Locality.localityid == id)
                .Select(t => t)
                .ToListAsync();
            return tt;
        }

        [HttpGet]
        [Route("/api/Schedule/last/{locid}")]
        public async Task<Schedule> GetLast(int locid)
        {
            return await _context.schedule
                .Include(s => s.Contract_Locality)
                .Where(s => s.Contract_Locality.localityid == locid)
                .Select(s => s)
                .OrderBy(s => s.id)
                .LastAsync();
        }

        [HttpGet]
        [Route("/api/Schedule/one/{conlocid}")]
        public async Task<Schedule> GetOne(int conlocid)
        {
            var schedule = new Schedule();

            foreach(var sc in await _context.schedule.Include(sc => sc.Contract_Locality).ThenInclude(cl => cl.Contract).ToListAsync())
                if (sc.Contract_Locality.id == conlocid)
                    schedule = sc;

            return schedule;
        }


        [HttpGet]
        [Route("/api/Schedule/task/{id}")]
        public async Task<Schedule> GetFromTaskMonthId(int id)
        {
            return await _context.taskmonth
                .Include(t => t.Schedule)
                .ThenInclude(s => s.Contract_Locality)
                .ThenInclude(sc => sc.Locality)
                .Where(t => t.id == id)
                .Select(t => t.Schedule)
                .FirstAsync();
        }

        [HttpPost]
        [Route("/api/Schedule/add")]
        public async Task Post([FromBody] Schedule value)
        {
            var conloc = await _context.contract_locality
                .Include(cl => cl.Contract)
                .Include(cl => cl.Locality)
                .Where(cl => cl.id == value.contract_localityid)
                .FirstOrDefaultAsync();

            //var con = await _context.contract_locality
            //    .Include(cl => cl.Contract)
            //    .Where(cl => cl.localityid == loc.id && cl.Contract.dateconclusion <= value.dateapproval && cl.Contract.validityperiod >= value.dateapproval)
            //    .Select(cl => cl.Contract)
            //    .FirstOrDefaultAsync();

            int countSchedule = 0;
            if (value.contract_localityid != 0)
            {
                countSchedule = await _context.schedule
                    .Where(sc => sc.contract_localityid == value.contract_localityid)
                    .CountAsync();
            }

            if (value.contract_localityid == 0)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"В дату {value.dateapproval.ToString("D")} для данного населенного пункта нет действующего контракта.");
            }
            else if (countSchedule == 0)
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
