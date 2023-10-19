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
/*using System.Web.Mvc;*/
/*using ClosedXML.Excel;*/

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

        //[HttpGet]
        //[Route("/api/Reports/{startDate}/{endDate}/{munid}")]
        //public async Task<double> Get(string startDate, string endDate, int munid)
        //{
        //    // выбираю нужные населенные пункты
        //    var needLocalities = await _context.municipality_locality.Select(l => l).Where(m => m.munid == munid).Select(h => h.localityid).ToListAsync();
        //    DateTime startdate = DateTime.Parse(startDate);
        //    DateTime enddate = DateTime.Parse(endDate);
        //    // то что фактически получилось, я пробегаюсь по всем актам отлова из этого нас пункта и считаю цену потом складываю
        //    return await _context.actcapture
        //        .Include(act => act.Locality)
        //        .Where(act => act.datecapture.Year >= startdate.Year
        //        && act.datecapture.Month >= startdate.Month
        //        && act.datecapture.Day >= startdate.Day
        //        && act.datecapture.Year <= enddate.Year
        //        && act.datecapture.Month <= enddate.Month
        //        && act.datecapture.Day <= enddate.Day
        //        && needLocalities.Contains(act.localityid))
        //        .SumAsync(act => act.Locality.tariph);
        //}

        //[HttpGet]
        //[Route("/api/Reports/{munid}")]
        //public async Task<Dictionary<int, int>> Get(int munid)
        //{
        //    // выбираю нужные населенные пункты
        //    var needLocalities = await _context.municipality_locality.Select(aa => aa).Where(aa => aa.munid == munid).Select(h => h.localityid).ToListAsync();

        //    //считаю по планам-графикам то что запланировано
        //    var countPlanAnimal = await _context.schedule
        //        .Include(sch => sch.TaskMonth)
        //        .Where(sch => needLocalities.Contains(sch.localityid))
        //        .SumAsync(sch => sch.TaskMonth.countanimal);

        //    // считаю по актам отлова то что в итоге
        //    var countAnimal = await _context.actcapture
        //        .Where(act => needLocalities.Contains(act.localityid))
        //        .CountAsync();

        //    return new Dictionary<int, int> { { countPlanAnimal, countAnimal } };
        //}

        [HttpGet]
        [Route("/api/Reports/money/export")]
        public FileStreamResult GetExcel()
        {
            return Export();
        }

        private FileStreamResult Export()
        {
            Workbook workbook = JsontoExcel();
            var stream = new MemoryStream();

           string fileName =  "test_out.xls"; 

            workbook.Save(stream, SaveFormat.Xlsx);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private Workbook JsontoExcel()
        {
            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];

           // string jsonInput = new WebClient().DownloadString(url);
            string s = "11.03.2023";
            string e = "12.06.2023";
            int id = 1;
            string jsonString = JsonSerializer.Serialize(Get(s, e, id));

            JsonLayoutOptions options = new JsonLayoutOptions();
            options.ArrayAsTable = true;


            JsonUtility.ImportData(jsonString, worksheet.Cells, 0, 0, options);

            return workbook;
        }
    }
}
