using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Organization")]
    public class OrganizationController
    {
        ApplicationContext _context;

        public OrganizationController(ApplicationContext context)
        {
            _context = context;
        }

        // вывести все организации
        [HttpGet]
        public async Task<IEnumerable<Organization>> Get()
        {
            return await _context.organization.ToListAsync();
        }

        // добавить новую организацию
        [HttpPost]
        [Route("/api/Organization/add")]
        public async Task Post([FromBody] Organization value)
        {
            await _context.organization.AddAsync(value);
            await _context.SaveChangesAsync();
        }
    }
}
