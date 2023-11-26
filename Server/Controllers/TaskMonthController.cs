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

        // вывести все задания на месяц

        [HttpGet]
        public async Task<IEnumerable<TaskMonth>> Get()
        {
            return await _context.taskmonth.ToListAsync();
        }

        // вывести задания на месяц
        [HttpGet("{id}/{conid}")]
        public async Task<IEnumerable<TaskMonth>> Get(int id, int conid)
        {
            return await _context.taskmonth
                .Include(t => t.Schedule)
                .Where(t => t.Schedule.localityid == id && t.Schedule.contractid == conid)
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

        // добавить новое задание на месяц
        [HttpPost]
        [Route("/api/TaskMonth/add")]
        public async Task Post([FromBody] TaskMonth value)
        {
            await _context.taskmonth.AddAsync(value);
            await _context.SaveChangesAsync();
        }

        // изменить задание на месяц
        [HttpPut]
        [Route("/api/TaskMonth/put/{id}")]
        public async Task Put(int id, [FromBody] TaskMonth value)
        {
            var currentTask = await _context.taskmonth.FirstOrDefaultAsync(t => t.id == id);
            if (currentTask != null)
            {
                currentTask.startdate = value.startdate;
                currentTask.enddate = value.enddate;
                currentTask.countanimal = value.countanimal;
                await _context.SaveChangesAsync();
            }
        }

        // удалить задание на месяц
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
