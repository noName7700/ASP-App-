using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/TaskMonth")]
    public class TaskMonthController : Controller
    {
        ApplicationContext _context;

        public TaskMonthController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<TaskMonth>> Get()
        {
            return await _context.taskmonth.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<TaskMonth>> Get(int id)
        {
            return await _context.taskmonth
                .Include(t => t.Schedule)
                .ThenInclude(s => s.Contract_Locality)
                .Where(t => t.Schedule.Contract_Locality.id == id)
                .Select(t => t)
                .ToListAsync();
        }

        [HttpGet]
        [Route("/api/TaskMonth/last")]
        public async Task<TaskMonth> GetLast()
        {
            return await _context.taskmonth
                .Include(t => t.Schedule)
                .Select(t => t)
                .OrderBy(t => t.id)
                .LastAsync();
        }

        [HttpGet]
        [Route("/api/TaskMonth/one/{id}")]
        public async Task<TaskMonth> GetOne(int id)
        {
            return await _context.taskmonth
                .Where(t => t.id == id)
                .Select(t => t)
                .FirstAsync();
        }

        [HttpPost]
        [Route("/api/TaskMonth/add")]
        public async Task Post([FromBody] TaskMonth value)
        {
            Contract_Locality contr = null;
            if (value.scheduleid != 0)
            {
                var sched = await _context.schedule
                    .Include(sc => sc.Contract_Locality)
                    .ThenInclude(cl => cl.Contract)
                    .Where(sc => sc.id == value.scheduleid)
                    .FirstOrDefaultAsync();
                contr = sched.Contract_Locality;
            }

            if (value != null && value.scheduleid != 0 && value.startdate >= value.enddate)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Дата начала не может быть позже даты окончания.");
            }
            else if (contr.Contract.validityperiod < value.startdate || contr.Contract.validityperiod < value.enddate ||
                contr.Contract.dateconclusion > value.startdate || contr.Contract.dateconclusion > value.enddate)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Для данного населенного пункта в дату {value.startdate.ToString("dd.MM.yyyy")} контракт не действовал.");
            }
            else if (value != null && value.scheduleid != 0)
            {
                await _context.taskmonth.AddAsync(value);
                await _context.SaveChangesAsync();
            }
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Для данного населенного пункта в дату {value.startdate.ToString("dd.MM.yyyy")} нет действующего контракта.");
            }
        }

        [HttpPut]
        [Route("/api/TaskMonth/put/{id}")]
        public async Task Put(int id, [FromBody] TaskMonth value)
        {
            var currentTask = await _context.taskmonth.FirstOrDefaultAsync(t => t.id == id);
            if (currentTask != null && value.startdate <= value.enddate)
            {
                currentTask.startdate = value.startdate;
                currentTask.enddate = value.enddate;
                currentTask.countanimal = value.countanimal;
                await _context.SaveChangesAsync();
            }
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Дата начала не может быть позже даты окончания.");
            }
        }

        [HttpDelete]
        [Route("/api/TaskMonth/delete/{id}")]
        public async Task Delete(int id)
        {
            var currentTask = await _context.taskmonth.FirstOrDefaultAsync(t => t.id == id);
            if (currentTask != null)
            {
                _context.taskmonth.Remove(currentTask);
                await _context.SaveChangesAsync();
            }
        }
    }
}
