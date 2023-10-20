using Microsoft.AspNetCore.Mvc;
using Server.Application;
using Domain;
using Microsoft.EntityFrameworkCore;

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
            return await _context.schedule.Where(sch => sch.id == id).Select(sch => sch.TaskMonth).ToListAsync();
        }

        [HttpGet]
        [Route("/api/Schedule/last/{locid}")]
        public async Task<Schedule> GetLast(int locid)
        {
            return await _context.schedule.Where(s => s.localityid == locid).Select(s => s).OrderBy(s => s.id).LastAsync();
        }

        [HttpGet]
        [Route("/api/Schedule/task/{id}")]
        public async Task<Schedule> GetFromTaskMonthId(int id)
        {
            return await _context.schedule.Where(s => s.taskmonthid == id).Select(s => s).FirstAsync();
        }

        // добавить новый план-график
        [HttpPost]
        [Route("/api/Schedule/add")]
        public async Task Post([FromBody] Schedule value)
        {
            await _context.schedule.AddAsync(value);
            await _context.SaveChangesAsync();
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
