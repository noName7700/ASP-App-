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
        [HttpGet(Name = "GetSchedules")]
        public IEnumerable<Schedule> Get()
        {
            // вывод планов графиков в виде: нас.пункт(название) - дата подписи
            var schedule_exemp = _context.schedule
                .Include(sch => sch.Locality)
                .GroupBy(sch => new { sch.Locality.name, sch.dateapproval })
                .OrderByDescending(sch => sch.Key.name)
                .Select(f => f.First())
                .ToList();

            return schedule_exemp;
        }

        // получить планы-графики относящиеся к одному нас. пункту
        [HttpGet("{id}")]
        public IEnumerable<Schedule> Get(int id)
        {
            // тут возвращаю планы-графики относящиеся к одному нас.пункту
            // несколько, потому что несколько taskmonth у нас
            return _context.schedule
                .Include(sch => sch.Locality)
                .Include(sch => sch.TaskMonth)
                .Select(sch => sch)
                .Where(s => s.localityid == id)
                .ToList();
        }

        // добавить новый план-график
        [HttpPost]
        public void Post([FromBody] Schedule value)
        {
            _context.schedule.Add(value);
            _context.SaveChanges();
        }

        // удалить выбранное задание на месяц (т.е. строку из таблицы планов-графиков)
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var currentSch = _context.schedule.FirstOrDefault(s => s.taskmonthid == id);
            if (currentSch != null)
            {
                _context.schedule.Remove(currentSch);
                _context.SaveChanges();
            }
        }

        // изменять тут можно только задания на месяц, но это уже сделано в TaskMonthController
        // можно добавить план-график (тут план-график это одна строка с одним заданием на месяц)
    }
}
