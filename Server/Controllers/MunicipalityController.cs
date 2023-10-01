﻿using Microsoft.AspNetCore.Mvc;
using Server.Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Municipality")]
    public class MunicipalityController
    {
        ApplicationContext _context;

        public MunicipalityController(ApplicationContext context)
        {
            _context = context;
        }

        // все муниципалитеты
        [HttpGet]
        public async Task<IEnumerable<MunicipalityName>> Get()
        {
           var t = await _context.municipalityname
                .ToListAsync();
            return t;
        }

/*        // вывести нас пункты одного муниципалитета
        [HttpGet("{id}")]
        public async Task<List<Locality>> Get(int id)
        {
            return await _context.municipality
                .Include(m => m.Locality)
                .Include(m => m.MunicipalityName)
                .Where(m => m.munid == id)
                .Select(m => m.Locality)
                .ToListAsync();
        }*/

        //[HttpGet("{id}")]
        //public async Task<IEnumerable<Locality>> Get(int id)
        //{
        //    string nameCurrentMunicipality = _context.municipality.Where(m => m.id == id).Select(m => m.name).FirstOrDefault();

        //    return await _context.municipality
        //        .Include(m => m.Locality)
        //        .Include(m => m.Contract)
        //        .Select(m => m.Locality)
        //        .Where(m => m.name == nameCurrentMunicipality)
        //        .ToListAsync();
        //}

        // создать новый муниципалитет
        [HttpPost]
        public async Task Post([FromBody] Municipality value)
        {
            await _context.municipality.AddAsync(value);
            await _context.SaveChangesAsync();
        }
    }
}