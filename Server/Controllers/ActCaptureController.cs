﻿using DocumentFormat.OpenXml.Spreadsheet;
using Domain;
using Domain.NonDomain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;
using System.Linq;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/ActCapture")]
    public class ActCaptureController : Controller, IRegister<ActCapture>
    {
        ApplicationContext _context;
        private readonly IRegister<ActCapture> proxy;
        public ActCaptureController(ApplicationContext context)
        {
            _context = context;
            proxy = new ActCaptureFilterProxy(this);
        }

        public async Task<List<ActCapture>> GetAll(Usercapture user, int id = 1)
        {
            var con_loc = await _context.contract_locality
                .Include(cl => cl.Contract)
                .Include(cl => cl.Locality)
                .Where(cl => cl.id == id)
                .FirstOrDefaultAsync();

            return await _context.actcapture
                .Include(a => a.Locality)
                .Where(a => a.contractid == con_loc.Contract.id && a.localityid == con_loc.Locality.id)
                .Select(a => a)
                .ToListAsync();
        }

        [HttpGet]
        public async Task<IEnumerable<Locality>> Get()
        {
            // выводятся только наши нас пункты
            return await _context.locality
                .ToListAsync();
        }

        [HttpGet("{locid}")]
        public async Task<IEnumerable<ActCapture>> Get(int locid)
        {
            return await _context.actcapture
                .Include(a => a.Locality)
                .Where(a => a.localityid == locid)
                .Select(a => a)
                .ToListAsync();
        }

        [HttpGet]
        [Route("/api/ActCapture/all/{conlocid}/{userid}")]
        public async Task<IEnumerable<ActCapture>> GetAll(int conlocid, int userid)
        {
            var user = await _context.usercapture
                .Where(u => u.id == userid)
                .FirstOrDefaultAsync();
            return await proxy.GetAll(user, conlocid);
        }

        [HttpGet]
        [Route("/api/ActCapture/last")]
        public async Task<ActCapture> GetLast()
        {
            return await _context.actcapture
                .Include(a => a.Locality)
                .Select(t => t)
                .OrderBy(t => t.id)
                .LastAsync();
        }

        [HttpGet]
        [Route("/api/ActCapture/one/{id}")]
        public async Task<ActCapture> GetOne(int id)
        {
            return await _context.actcapture
                .Where(a => a.id == id)
                .Select(a => a)
                .FirstOrDefaultAsync();
        }

        [HttpGet]
        [Route("/api/ActCapture/animal/{id}")]
        public async Task<ActCapture> GetFromAnimalId(int id)
        {
            return await _context.animal
                .Include(a => a.ActCapture)
                .Where(a => a.id == id)
                .Select(a => a.ActCapture)
                .FirstAsync();
        }

        [HttpGet]
        [Route("/api/ActCapture/alll")]
        public async Task<IEnumerable<Animal>> GetFromScheduleId()
        {
            return await _context.animal
                .Include(a => a.ActCapture)
                .ToListAsync();
        }

        [HttpPost]
        [Route("/api/ActCapture/add")]
        public async Task Post([FromBody] ActCapture value)
        {
            var oldAct = await _context.actcapture
                .Where(a => a.datecapture == value.datecapture && a.localityid == value.localityid && a.contractid == value.contractid)
                .FirstOrDefaultAsync();

            var schedule = await _context.schedule
                .Where(s => s.id == value.scheduleid)
                .FirstOrDefaultAsync();
            TaskMonth taskmonth = new TaskMonth();
            if (schedule != null)
            {
                taskmonth = await _context.taskmonth
                    .Where(t => t.scheduleid == schedule.id && t.startdate <= value.datecapture && t.enddate >= value.datecapture)
                    .FirstOrDefaultAsync();
            }

            if (schedule == null)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"В дату {value.datecapture.ToString("dd.MM.yyyy")} нет действующего контракта.");
            }
            else if (taskmonth == null)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"В дату {value.datecapture.ToString("dd.MM.yyyy")} по плану-графику не проводился отлов.");
            }
            else if (oldAct != null)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"В дату {value.datecapture.ToString("dd.MM.yyyy")} уже есть акт отлова.");
            }
            else
            {
                await _context.actcapture.AddAsync(value);
                await _context.SaveChangesAsync();
            }
        }

        [HttpPut]
        [Route("/api/ActCapture/put/{id}")]
        public async Task Put(int id, [FromBody] ActCapture value)
        {
            var currentLoc = await _context.actcapture.FirstOrDefaultAsync(l => l.id == id);

            var oldAct = await _context.actcapture
                .Where(a => a.datecapture == value.datecapture && a.localityid == value.localityid && a.contractid == value.contractid)
                .FirstOrDefaultAsync();

            var schedule = await _context.schedule
                .Where(s => s.id == value.scheduleid)
                .FirstOrDefaultAsync();
            TaskMonth taskmonth = new TaskMonth();
            if (schedule != null)
            {
                taskmonth = await _context.taskmonth
                    .Where(t => t.scheduleid == schedule.id && t.startdate <= value.datecapture && t.enddate >= value.datecapture)
                    .FirstOrDefaultAsync();
            }

            if (currentLoc != null && schedule == null)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"В дату {value.datecapture.ToString("dd.MM.yyyy")} нет действующего контракта.");
            }
            else if (currentLoc != null && taskmonth == null)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"В дату {value.datecapture.ToString("dd.MM.yyyy")} по плану-графику не проводился отлов.");
            }
            else if (currentLoc != null && oldAct != null)
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"В дату {value.datecapture.ToString("dd.MM.yyyy")} уже есть акт отлова.");
            }
            else if (currentLoc != null)
            {
                currentLoc.datecapture = value.datecapture;
                currentLoc.localityid = value.localityid;
                currentLoc.contractid = value.contractid;
                await _context.SaveChangesAsync();
            }
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Введены неверные данные.");
            }
        }

        // удалить акт 
        [HttpDelete]
        [Route("/api/ActCapture/delete/{id}")]
        public async Task Delete(int id)
        {
            var currentAct = await _context.actcapture.FirstOrDefaultAsync(s => s.id == id);
            if (currentAct != null)
            {
                _context.actcapture.Remove(currentAct);
                await _context.SaveChangesAsync();
            }
            else
            {
                Response.StatusCode = 403;
                await Response.WriteAsync($"Не выбран акт отлова.");
            }
        }

    }
}
