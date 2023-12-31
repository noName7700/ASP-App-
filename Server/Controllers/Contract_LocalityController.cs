﻿using Microsoft.AspNetCore.Mvc;
using Server.Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using Domain.NonDomain;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Contract_Locality")]
    public class Contract_LocalityController : Controller, IRegister<Contract_Locality>
    {
        ApplicationContext _context;
        private readonly IRegister<Contract_Locality> proxy;
        public Contract_LocalityController(ApplicationContext context)
        {
            _context = context;
            proxy = new Contract_LocalityFilterProxy(this);
        }

        public async Task<List<Contract_Locality>> GetAll(Usercapture user, int id = 1)
        {
            return await _context.contract_locality
                 .Include(cl => cl.Organization)
                 .Include(cl => cl.Contract)
                 .Include(cl => cl.Locality)
                 .Where(cl => cl.contractid == id)
                 .Select(cl => cl)
                 .ToListAsync();
        }

        [HttpGet]
        public async Task<IEnumerable<Contract_Locality>> GetAll()
        {
            return await _context.contract_locality
                 .Include(cl => cl.Organization)
                 .Include(cl => cl.Contract)
                 .Include(cl => cl.Locality)
                 .Select(cl => cl)
                 .ToListAsync();
        }

        [HttpGet]
        [Route("/api/Contract_Locality/{id}/{userid}")]
        public async Task<IEnumerable<Contract_Locality>> Get(int id, int userid)
        {
            var user = await _context.usercapture
                .Where(u => u.id == userid)
                .FirstOrDefaultAsync();
            return await proxy.GetAll(user, id);
        }

        [HttpGet]
        [Route("/api/Contract_Locality/one/{id}")]
        public async Task<Contract_Locality> GetOne(int id)
        {
            var t = await _context.contract_locality
                .Include(cl => cl.Organization)
                .Include(cl => cl.Contract)
                .Include(cl => cl.Locality)
                .Where(cl => cl.localityid == id)
                .Select(cl => cl)
                .FirstOrDefaultAsync();
            return t;
        }

        [HttpGet]
        [Route("/api/Contract_Locality/one/id/{id}")]
        public async Task<Contract_Locality> GetOneId(int id)
        {
            var t = await _context.contract_locality
                .Include(cl => cl.Organization)
                .Include(cl => cl.Contract)
                .Include(cl => cl.Locality)
                .Where(cl => cl.id == id)
                .Select(cl => cl)
                .FirstOrDefaultAsync();
            return t;
        }

        [HttpGet]
        [Route("/api/Contract_Locality/date/{id}/{datecapture}")]
        public async Task<Contract_Locality> GetDate(int id, string datecapture)
        {
            DateTime date = DateTime.Parse(datecapture);

            foreach (var cl in await _context.contract_locality.Include(cl => cl.Contract).ToListAsync())
            {
                if (cl.localityid == id && cl.Contract.dateconclusion <= date && cl.Contract.validityperiod >= date)
                    return cl;
            }
            return new Contract_Locality();
        }

        [HttpPut]
        [Route("/api/Contract_Locality/put/{id}")]
        public async Task Put(int id, [FromBody] Contract_Locality value)
        {
            //var contractValue = await _context.contract_locality
            //    .Include(cl => cl.Contract)
            //    .Where(cl => cl.id == id)
            //    .Select(cl => cl.Contract)
            //    .FirstOrDefaultAsync();

            //var countContract = await _context.contract
            //    .Where(c => c.municipalityid == contractValue.municipalityid && c.validityperiod >= contractValue.dateconclusion)
            //    .CountAsync();

            var currentConLoc = await _context.contract_locality.FirstOrDefaultAsync(l => l.id == id);
            if (currentConLoc != null)
            {
                currentConLoc.tariph = value.tariph;
                await _context.SaveChangesAsync();
            }
            //else
            //{
            //    тут выводится ошибка
            //}
        }

        // добавить цену за животное в нас пункте по контракту
        [HttpPost]
        [Route("/api/Contract_Locality/add")]
        public async Task Post([FromBody] Contract_Locality value)
        {
            if (value.contractid == 0)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Данного контракта не существует.");
            }
            else if (value.localityid == 0)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Данного населенного пункта не существует.");
            }
            else if (value.organizationid == 0)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Данной организации не существует.");
            }
            else
            {
                await _context.contract_locality.AddAsync(value);
                await _context.SaveChangesAsync();
            }
        }
    }
}
