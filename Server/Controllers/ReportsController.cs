using Domain;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application;
using Aspose.Cells;
using Aspose.Cells.Utility;
using System.Net;
using System.IO;
using System.Linq;
using System.Web;
using System.Text.Json;
using System.Security.Cryptography;
using Newtonsoft.Json;
//using closedxml.excel;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Reports")]
    public class ReportsController : Controller
    {
        ApplicationContext _context;

        public ReportsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/api/Reports/{startDate}/{endDate}/{munid}")]
        public async Task<double> Get(string startDate, string endDate, int munid)
        {
            var needLocalities = await _context.locality.Select(l => l).Where(m => m.municipalityid == munid).Select(h => h.id).ToListAsync();
            DateTime startdate = DateTime.Parse(startDate);
            DateTime enddate = DateTime.Parse(endDate);

            // тут словарь: нас пункт id - тариф
            var loc_tar = await _context.contract_locality
                .Select(lc => new { lc.localityid, lc.tariph })
                .ToDictionaryAsync(lc => lc.localityid, lc => lc.tariph);


            // то что фактически получилось, я пробегаюсь по всем актам отлова из этого нас пункта и считаю цену потом складываю
            double summ = 0;
            foreach (var act in _context.actcapture)
            {
                if (act.datecapture >= startdate && act.datecapture <= enddate 
                    && needLocalities.Contains(act.localityid))
                {
                    summ += loc_tar[act.localityid];
                }
            }

            return summ;

            // почему Linq не работает

            //var t = await _context.actcapture
            //    .Where(act => act.datecapture.Year >= startdate.Year
            //    && act.datecapture.Month >= startdate.Month
            //    && act.datecapture.Day >= startdate.Day
            //    && act.datecapture.Year <= enddate.Year
            //    && act.datecapture.Month <= enddate.Month
            //    && act.datecapture.Day <= enddate.Day
            //    && needLocalities.Contains(act.localityid))
            //    .SumAsync(act => loc_tar[act.localityid]);
        }

        [HttpGet]
        [Route("/api/Reports/{munid}/{locid}")]
        public async Task<Dictionary<int, int>> Get(int munid, int locid)
        {
            //считаю по планам-графикам то что запланировано
            var countPlanAnimal = await _context.taskmonth
                .Include(sch => sch.Schedule)
                .Where(sch => sch.Schedule.localityid == locid)
                .SumAsync(t => t.countanimal);

            // считаю по актам отлова то что в итоге
            var countAnimal = await _context.animal
                .Include(an => an.ActCapture)
                .Where(an => an.ActCapture.localityid == locid)
                .CountAsync();

            return new Dictionary<int, int> { { countPlanAnimal, countAnimal } };
        }

        [HttpGet]
        [Route("/api/Reports/money/export/{d}")]
        public FileStreamResult GetExcel(double d)
        {
            return Export(d);
        }

        [HttpGet]
        [Route("/api/Reports/schedule/export/{d}")]
        public FileStreamResult GetExcel(Dictionary<int, int> d)
        {
            return Export(d);
        }

        private FileStreamResult Export(Dictionary<int, int> d)
        {
            Workbook workbook = JsontoExcel(d);
            var stream = new MemoryStream();

            string fileName = "test_out.xls";

            workbook.Save(stream, SaveFormat.Xlsx);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private FileStreamResult Export(double d)
        {
            Workbook workbook = JsontoExcel(d);
            var stream = new MemoryStream();

            string fileName = "test_out.xls";

            workbook.Save(stream, SaveFormat.Xlsx);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private Workbook JsontoExcel(Dictionary<int, int> d)
        {
            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];

            //var Animals = new Dictionary<int, int>()
            //{
            //    [5] = 6,
            //    [6] = 8,
            //    [7] = 9
            //};

            //"Переворачивает" словарь, чтобы он отображался корректно(по столбам, а не строкам)
            var trans = from key in d.Keys
                              select new { Животные = key, ТожеЖивотные = d[key] };

            
            string jsonInput = JsonConvert.SerializeObject(trans);

            JsonLayoutOptions options = new JsonLayoutOptions();
            options.ArrayAsTable = true;

            //Удаление рекламной страницы
            //workbook.Worksheets.RemoveAt(1);
            //JsonUtility.ImportData(jsonString, worksheet.Cells, 0, 0, options);

            return workbook;
        }

        private Workbook JsontoExcel(double d)
        {
            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];

            //double a = 2.5

            

            JsonLayoutOptions options = new JsonLayoutOptions();
            options.ArrayAsTable = true;

            //Удаление рекламной страницы
            //workbook.Worksheets.RemoveAt(1);
            JsonUtility.ImportData("НачалоПериода", worksheet.Cells, 0, 1, options);
            JsonUtility.ImportData("КонецПериода", worksheet.Cells, 0, 2, options);
            JsonUtility.ImportData("Сумма", worksheet.Cells, 0, 0, options);
            JsonUtility.ImportData(d.ToString(), worksheet.Cells, 1, 0, options);

            return workbook;
        }
    }
}
