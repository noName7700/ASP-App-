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
        public IEnumerable<TaskMonth> Get()
        {
            return _context.taskmonth.ToList();
        }

        // вывести одно задание на месяц
        [HttpGet("{id}")]
        public TaskMonth Get(int id)
        {
            return _context.taskmonth.FirstOrDefault(t => t.id == id);
        }

        // добавить новое задание на месяц
        [HttpPost]
        public void Post([FromBody] TaskMonth value)
        {
            _context.taskmonth.Add(value);
            _context.SaveChanges();
        }

        // изменить задание на месяц
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] TaskMonth value)
        {
            var currentTask = _context.taskmonth.FirstOrDefault(t => t.id == id);
            if (currentTask != null)
            {
                currentTask.startdate = value.startdate;
                currentTask.enddate = value.enddate;
                currentTask.countanimal = value.countanimal;
                _context.SaveChanges();
            }
        }

        // удалить задание на месяц
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var currentTask = _context.taskmonth.FirstOrDefault(t => t.id == id);
            if (currentTask != null)
            {
                _context.taskmonth.Remove(currentTask);
                _context.SaveChanges();
            }
        }
    }
}
