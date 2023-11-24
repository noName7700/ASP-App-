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
using System.Text;
using System.Formats.Asn1;
//using CsvHelper;
using ClosedXML.Excel;
using System.IO;
using System.Globalization;
using DocumentFormat.OpenXml.Spreadsheet;
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
        [Route("/api/Reports/money/{startDate}/{endDate}/{munid}")]
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
        [Route("/api/Reports/schedule/{startDate}/{endDate}/{munid}/{locid}")]
        public async Task<Dictionary<int, int>> Get(string startDate, string endDate, int munid, int locid)
        {
            DateTime startdate = DateTime.Parse(startDate);
            DateTime enddate = DateTime.Parse(endDate);

            //считаю по планам-графикам то что запланировано
            int summPlan = 0;
            foreach (var task in _context.taskmonth.Include(t => t.Schedule))
                if (task.Schedule.localityid == locid && task.enddate >= startdate && task.enddate <= enddate)
                    summPlan += task.countanimal;


            //var countPlanAnimal = await _context.taskmonth
            //    .Include(t => t.Schedule)
            //    .Where(t => t.Schedule.localityid == locid && t.enddate >= startdate && t.enddate <= enddate)
            //    .SumAsync(t => t.countanimal);

            // считаю по актам отлова то что в итоге
            int countAn = 0;
            foreach (var an in _context.animal.Include(a => a.ActCapture))
                if (an.ActCapture.localityid == locid && an.ActCapture.datecapture >= startdate && an.ActCapture.datecapture <= enddate)
                    countAn += 1;

            //var countAnimal = await _context.animal
            //    .Include(an => an.ActCapture)
            //    .Where(an => an.ActCapture.localityid == locid && an.ActCapture.datecapture >= startdate && an.ActCapture.datecapture <= enddate)
            //    .CountAsync();

            return new Dictionary<int, int> { { summPlan, countAn } };
        }

        [HttpGet]
        [Route("/api/Reports/money/export/{startdate}/{enddate}/{munid}/{d}")]
        public async Task<FileResult> GetExcelMoney(string startdate, string enddate, int munid, double d)
        {
            Municipality mun = await _context.municipality
                .Where(m => m.id == munid)
                .Select(m => m)
                .FirstOrDefaultAsync();

            var sdate = DateTime.Parse(startdate);
            var edate = DateTime.Parse(enddate);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add();
                worksheet.Cell("A1").Value = "Отчет о выполненной работе за контракт";
                var title = worksheet.Range("A1:B1");
                title.Merge().Style.Font.SetBold().Font.FontSize = 13;
                worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A2").Value = "Муниципалитет:";
                worksheet.Cell("A2").Style.Font.SetBold().Font.FontSize = 12;

                worksheet.Cell("B2").Value = $"{mun.name}";
                worksheet.Cell("B2").Style.Font.FontSize = 12;

                worksheet.Cell("A3").Value = "В период";
                title = worksheet.Range("A3:B3");
                title.Merge().Style.Font.SetBold().Font.FontSize = 12;
                worksheet.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A4").Value = $"с {sdate.ToString("dd.MM.yyyy")}";
                worksheet.Cell("A4").Style.Font.FontSize = 12;

                worksheet.Cell("B4").Value = $"до {edate.ToString("dd.MM.yyyy")}";
                worksheet.Cell("B4").Style.Font.FontSize = 12;

                worksheet.Cell("A5").Value = $"Получена сумма:";
                worksheet.Cell("A5").Style.Font.SetBold().Font.FontSize = 12;

                worksheet.Cell("B5").Value = $"{d}₽";
                worksheet.Cell("B5").Style.Font.SetBold().Font.FontSize = 12;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{DateTime.Now}.xlsx");
                }
            }
        }

        [HttpGet]
        [Route("/api/Reports/money/export/{startdate}/{enddate}/{munid}/{locid}/{plan}/{fact}")]
        public async Task<FileResult> GetExcelSchedule(string startdate, string enddate, int munid, int locid, int plan, int fact)
        {
            Municipality mun = await _context.municipality
                .Where(m => m.id == munid)
                .Select(m => m)
                .FirstOrDefaultAsync();

            Locality loc = await _context.locality
                .Where(m => m.id == locid)
                .Select(m => m)
                .FirstOrDefaultAsync();

            var sdate = DateTime.Parse(startdate);
            var edate = DateTime.Parse(enddate);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add();
                worksheet.Cell("A1").Value = "Отчет о выполненной работе по планам-графикам";
                var title = worksheet.Range("A1:B1");
                title.Merge().Style.Font.SetBold().Font.FontSize = 13;
                worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A2").Value = "Муниципалитет:";
                worksheet.Cell("A2").Style.Font.SetBold().Font.FontSize = 12;

                worksheet.Cell("B2").Value = $"{mun.name}";
                worksheet.Cell("B2").Style.Font.FontSize = 12;

                worksheet.Cell("A3").Value = "Населенный пункт:";
                worksheet.Cell("A3").Style.Font.SetBold().Font.FontSize = 12;

                worksheet.Cell("B3").Value = $"{loc.name}";
                worksheet.Cell("B3").Style.Font.FontSize = 12;

                worksheet.Cell("A4").Value = "В период";
                title = worksheet.Range("A4:B4");
                title.Merge().Style.Font.SetBold().Font.FontSize = 12;
                worksheet.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A5").Value = $"с {sdate.ToString("dd.MM.yyyy")}";
                worksheet.Cell("A5").Style.Font.FontSize = 12;

                worksheet.Cell("B5").Value = $"до {edate.ToString("dd.MM.yyyy")}";
                worksheet.Cell("B5").Style.Font.FontSize = 12;

                worksheet.Cell("A6").Value = "Запланированное количество:";
                worksheet.Cell("A6").Style.Font.FontSize = 12;

                worksheet.Cell("B6").Value = $"{plan}";
                worksheet.Cell("B6").Style.Font.FontSize = 12;

                worksheet.Cell("A7").Value = "Количество по факту:";
                worksheet.Cell("A7").Style.Font.FontSize = 12;

                worksheet.Cell("B7").Value = $"{fact}";
                worksheet.Cell("B7").Style.Font.FontSize = 12;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"fff.xlsx");
                }
            }
        }
    }
}
