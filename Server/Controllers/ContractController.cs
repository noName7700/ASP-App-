using Microsoft.AspNetCore.Mvc;
using Server.Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Contract")]
    public class ContractController : Controller
    {
        ApplicationContext _context;

        public ContractController(ApplicationContext context)
        {
            _context = context;
        }

        // получить все контракты
        [HttpGet]
        public async Task<IEnumerable<Contract>> Get()
        {
            return await _context.contract
                .Include(c => c.Municipality)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<int> Get(int id)
        {
            return await _context.contract
                .Where(c => c.municipalityid == id)
                .Select(c => c.id)
                .FirstOrDefaultAsync();
        }

        [HttpGet]
        [Route("/api/Contract/all/{id}")]
        public async Task<IEnumerable<Contract>> GetAll(int id)
        {
            return await _context.contract
                .Include(c => c.Municipality)
                .Where(c => c.municipalityid == id)
                .Select(c => c)
                .ToListAsync();
        }

        [HttpGet]
        [Route("/api/Contract/last")]
        public async Task<Contract> GetLast()
        {
            return await _context.contract
                .Include(c => c.Municipality)
                .Select(t => t)
                .OrderBy(t => t.id)
                .LastAsync();
        }

        [HttpGet]
        [Route("/api/Contract/one/{id}")]
        public async Task<Contract> GetOne(int id)
        {
            return await _context.contract
                .Include(c => c.Municipality)
                .FirstOrDefaultAsync(t => t.id == id);
        }

        // создать контракт (т.е. одну запись)
        [HttpPost]
        [Route("/api/Contract/add")]
        public async Task Post([FromBody] Contract value)
        {
            // проверка, что нет контракта с этим муниципалитетом
            // что дата действия прошлого не больше даты утверждения нового контракта
            var countContract = await _context.contract
                .Where(c => c.municipalityid == value.municipalityid && c.validityperiod >= value.dateconclusion)
                .CountAsync();
            if (countContract == 0)
            {
                await _context.contract.AddAsync(value);
                await _context.SaveChangesAsync();
            }
            //else
            //{
            //    тут что-то отправляю типа ошибка "Нельзя добавить новый контрак, т.к. дата действия предыдущего еще не истекла" 
            //}
        }

        [HttpPut]
        [Route("/api/Contract/put/{id}")]
        public async Task Put(int id, [FromBody] Contract value)
        {
            var countContract = await _context.contract
                .Where(c => c.municipalityid == value.municipalityid && c.validityperiod >= value.dateconclusion)
                .CountAsync();

            var currentContract = await _context.contract.FirstOrDefaultAsync(t => t.id == id);
            if (currentContract != null || countContract == 0)
            {
                currentContract.validityperiod = value.validityperiod;
                currentContract.dateconclusion = value.dateconclusion;
                await _context.SaveChangesAsync();
            }
            //else
            //{
            //    ошибка
            //}
        }

        // удалить контракт по id
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var currentCon = await _context.contract.FirstOrDefaultAsync(c => c.id == id);
            if (currentCon != null)
            {
                _context.contract.Remove(currentCon);
                await _context.SaveChangesAsync();
            }
        }
    }
}
