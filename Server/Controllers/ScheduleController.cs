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
            // вывод планов графиков в виде: нас.пункт(название) - дата подписи
            return await _context.schedule
                .Include(sch => sch.Locality)
                .GroupBy(sch => new { sch.Locality.name, sch.dateapproval })
                .OrderByDescending(sch => sch.Key.name)
                .Select(f => f.First())
                .ToListAsync();
        }

        /*// получить планы-графики относящиеся к одному нас. пункту
        [HttpGet("{id}")]
        public async Task<IEnumerable<Schedule>> Get(int id)
        {
            // тут возвращаю планы-графики относящиеся к одному нас.пункту
            // несколько, потому что несколько taskmonth у нас
            return await _context.schedule
                .Include(sch => sch.Locality)
                .Include(sch => sch.TaskMonth)
                .Select(sch => sch)
                .Where(s => s.localityid == id)
                .ToListAsync();
        }*/

        // вывести задания на месяц по id плана-графика
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

            foreach(var sc in await _context.schedule.ToListAsync())
            {
                if (sc.localityid == locid && sc.dateapproval <= startDate)
                    schedule = sc;
            }

            return schedule;
            //return await _context.schedule
            //    .Where(s => s.localityid == locid && s.dateapproval <= startDate)
            //    .Select(s => s)
            //    .FirstOrDefaultAsync();
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

        // добавить новый план-график
        [HttpPost]
        [Route("/api/Schedule/add")]
        public async Task Post([FromBody] Schedule value)
        {
            // тут я проверяю, что если мы добавляем план-график для контракта
            // для которого уже есть контракт, то мы выводим ошибку
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
                await Response.WriteAsync("Так нельзя((");
            }
        }

        // удалить выбранное задание на месяц (т.е. строку из таблицы планов-графиков)
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

        // изменять тут можно только задания на месяц, но это уже сделано в TaskMonthController
        // можно добавить план-график (тут план-график это одна строка с одним заданием на месяц)
    }
}
