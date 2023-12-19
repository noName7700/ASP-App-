using Microsoft.AspNetCore.Mvc;
using Server.Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using Domain.NonDomain;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Contract")]
    public class ContractController : Controller, IRegister<Contract>
    {
        ApplicationContext _context;
        private readonly IRegister<Contract> proxy;
        public ContractController(ApplicationContext context)
        {
            _context = context;
            proxy = new ContractFilterProxy(this);
        }

        public async Task<List<Contract>> GetAll(Usercapture user, int id = 1)
        {
            return await _context.contract
                .Include(c => c.Municipality)
                .ToListAsync();
        }

        // получить все контракты
        [HttpGet]
        [Route("/api/Contract/user/{userid}")]
        public async Task<IEnumerable<Contract>> GetAllUser(int userid)
        {
            var user = await _context.usercapture
                .Where(u => u.id == userid)
                .FirstOrDefaultAsync();
            return await proxy.GetAll(user);
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
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync("Нельзя добавить новый контракт, так как дата действия предыдущего еще не истекла");
            }
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
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync("Нельзя добавить новый контракт, так как дата действия предыдущего еще не истекла");
            }
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
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync("Контракт не выбран.");
            }
        }
    }
}
