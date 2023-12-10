﻿using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;
using System.Text.RegularExpressions;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Organization")]
    public class OrganizationController : Controller
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
            return await _context.organization
                .Include(o => o.Locality)
                .ToListAsync();
        }

        [HttpGet]
        [Route("/api/Organization/one/{id}")]
        public async Task<Organization> GetOne(int id)
        {
            return await _context.organization
                .Include(o => o.Locality)
                .FirstOrDefaultAsync(o => o.id == id);
        }

        [HttpGet]
        [Route("/api/Organization/one/loc/{id}")]
        public async Task<IEnumerable<Organization>> GetOneFromLocId(int id)
        {
            return await _context.organization
                .Include(o => o.Locality)
                .Where(o => o.localityid == id)
                .ToListAsync();
        }

        [HttpGet]
        [Route("/api/Organization/last")]
        public async Task<Organization> GetLast()
        {
            return await _context.organization
                .Include(o => o.Locality)
                .Select(t => t)
                .OrderBy(t => t.id)
                .LastAsync();
        }

        // добавить новую организацию
        [HttpPost]
        [Route("/api/Organization/add")]
        public async Task Post([FromBody] Organization value)
        {
            if (long.TryParse(value.telephone, out long n) && value.telephone.Length != 11)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Номер телефона должен состоять только из 11 цифр.");
            }
            else
            {
                await _context.organization.AddAsync(value);
                await _context.SaveChangesAsync();
            }
        }

        [HttpPut]
        [Route("/api/Organization/put/{id}")]
        public async Task Put(int id, [FromBody] Organization value)
        {
            var currentOrganization = await _context.organization.FirstOrDefaultAsync(t => t.id == id);
            if (currentOrganization != null && long.TryParse(value.telephone, out long t) && value.telephone.Length != 11)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Номер телефона должен состоять только из 11 цифр.");
            }    
            else if (currentOrganization != null)
            {
                currentOrganization.name = value.name;
                currentOrganization.telephone = value.telephone;
                currentOrganization.email = value.email;
                await _context.SaveChangesAsync();
            }
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Введены неверные данные.");
            }
        }

        [HttpDelete]
        [Route("/api/Organization/delete/{id}")]
        public async Task Delete(int id)
        {
            var currentOrg = await _context.organization.FirstOrDefaultAsync(c => c.id == id);
            if (currentOrg != null)
            {
                _context.organization.Remove(currentOrg);
                await _context.SaveChangesAsync();
            }
        }
    }
}
