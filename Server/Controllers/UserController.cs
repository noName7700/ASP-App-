using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/User")]
    public class UserController : Controller
    {
        ApplicationContext _context;

        public UserController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Usercapture>> Get()
        {
            return await _context.usercapture
                .Include(u => u.Municipality)
                .Include(u => u.Locality)
                .Include(u => u.Organization)
                .ToListAsync();
        }

        [HttpGet]
        [Route("/api/User/last")]
        public async Task<Usercapture> GetLast()
        {
            return await _context.usercapture
                .Include(u => u.Municipality)
                .Include(u => u.Locality)
                .Include(u => u.Organization)
                .Select(t => t)
                .OrderBy(t => t.id)
                .LastAsync();
        }

        [HttpPost]
        [Route("/api/User/add")]
        public async Task Post([FromBody] Usercapture value)
        {
            await _context.usercapture.AddAsync(value);
            await _context.SaveChangesAsync();
        }
    }
}
