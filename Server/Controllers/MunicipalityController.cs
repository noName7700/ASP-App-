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
        public async Task<IEnumerable<Municipality>> Get()
        {
            return await _context.municipality
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<Municipality> Get(int id)
        {
            return await _context.municipality
                .Where(m => m.id == id)
                .Select (m => m)
                .FirstOrDefaultAsync();
        }

        [HttpGet]
        [Route("/api/Municipality/last")]
        public async Task<Municipality> GetLast()
        {
            return await _context.municipality
                .Select(t => t)
                .OrderBy(t => t.id)
                .LastAsync();
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
        [Route("/api/Municipality/add")]
        public async Task Post([FromBody] Municipality value)
        {
            var countMun = await _context.municipality
                .Where(m => m.name == value.name)
                .CountAsync();

            if (countMun == 0)
            {
                await _context.municipality.AddAsync(value);
                await _context.SaveChangesAsync();
            }
            //else
            //{
            //    ошибка
            //}
        }

        [HttpGet]
        [Route("/api/Municipality/loc/{id}")]
        public async Task<Municipality> GetFromLocalityId(int id)
        {
            return await _context.locality
                .Include(l => l.Municipality)
                .Where(l => l.id == id)
                .Select(m => m.Municipality)
                .FirstAsync();
        }

        //[HttpPost]
        //[Route("/api/Municipality/add-loc")]
        //public async Task Post([FromBody] Municipality_Locality value)
        //{
        //    await _context.municipality_locality.AddAsync(value);
        //    await _context.SaveChangesAsync();
        //}

        //[HttpDelete]
        //[Route("/api/Municipality/delete/{id}")]
        //public async Task Delete(int id)
        //{
        //    var currentMun = await _context.municipality_locality.FirstOrDefaultAsync(s => s.id == id);
        //    if (currentMun != null)
        //    {
        //        _context.municipality_locality.Remove(currentMun);
        //        await _context.SaveChangesAsync();
        //    }
        //}
    }
}
