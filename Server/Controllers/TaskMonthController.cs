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

        // вывести одно задание на месяц
        [HttpGet("{id}")]
        public async Task<TaskMonth> Get(int id)
        {
            return await _context.taskmonth.FirstOrDefaultAsync(t => t.id == id);
        }

        // добавить новое задание на месяц
        [HttpPost]
        public async Task Post([FromBody] TaskMonth value)
        {
            await _context.taskmonth.AddAsync(value);
            await _context.SaveChangesAsync();
        }

        // изменить задание на месяц
        [HttpPut("{id}")]
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
        [HttpDelete("{id}")]
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
