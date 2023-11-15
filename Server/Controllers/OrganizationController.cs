﻿using Domain;
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

        [HttpGet]
        [Route("/api/Organization/one/{id}")]
        public async Task<Organization> GetOne(int id)
        {
            return await _context.organization.FirstOrDefaultAsync(o => o.id == id);
        }

        // добавить новую организацию
        [HttpPost]
        [Route("/api/Organization/add")]
        public async Task Post([FromBody] Organization value)
        {
            await _context.organization.AddAsync(value);
            await _context.SaveChangesAsync();
        }

        [HttpPut]
        [Route("/api/Organization/put/{id}")]
        public async Task Put(int id, [FromBody] Organization value)
        {
            var currentOrganization = await _context.organization.FirstOrDefaultAsync(t => t.id == id);
            if (currentOrganization != null)
            {
                currentOrganization.name = value.name;
                currentOrganization.telephone = value.telephone;
                currentOrganization.email = value.email;
                await _context.SaveChangesAsync();
            }
        }

        // удалить контракт по id
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