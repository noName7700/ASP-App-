using Microsoft.AspNetCore.Mvc;
using Server.Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/Journal")]
    public class JournalController : ControllerBase
    {
        ApplicationContext _context;

        public JournalController(ApplicationContext context)
        {
            _context = context;
        }

        // получить данные журнала для таблицы
        [HttpGet("{id}")]
        //[Route("/api/Journal/{id}")]
        public async Task<IEnumerable<Journal>> Get(int id)
        {
            var aa = await _context.journal
                .Include(j => j.Usercapture)
                .ThenInclude(j => j.Organization)
                .Include(j => j.Usercapture)
                .ThenInclude(j => j.Role)
                .Where(j => j.nametable == id)
                .Select(j => j)
                .ToListAsync();
            return aa;
        }

        // добавить запись журнала
        [HttpPost]
        [Route("/api/Journal/add")]
        public async Task Post([FromBody] Journal value)
        {
            await _context.journal.AddAsync(value);
            await _context.SaveChangesAsync();
        }

        // удалить запись журнала
        [HttpDelete]
        [Route("/api/Journal/delete/{id}")]
        public async Task Delete(int id)
        {
            var currentJou = await _context.journal.FirstOrDefaultAsync(j => j.id == id);
            if (currentJou != null)
            {
                _context.journal.Remove(currentJou);
                await _context.SaveChangesAsync();
            }
        }

        [HttpGet]
        [Route("/api/Journal/export/{id}")]
        public async Task<FileResult> GetExcelJournal(int id)
        {
            var journals = await _context.journal
                .Include(j => j.Usercapture)
                .ThenInclude(j => j.Organization)
                .Include(j => j.Usercapture)
                .ThenInclude(j => j.Role)
                .Where(j => j.nametable == id)
                .Select(j => j)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add();
                worksheet.Cell("A1").Value = "Журнал изменений";
                var title = worksheet.Range("A1:C1");
                title.Merge().Style.Font.SetBold().Font.FontSize = 13;
                worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A2").Value = "ФИО пользователя";
                worksheet.Cell("A2").Style.Font.SetBold().Font.FontSize = 12;

                worksheet.Cell("B2").Value = "Дата изменения";
                worksheet.Cell("B2").Style.Font.SetBold().Font.FontSize = 12;

                worksheet.Cell("C2").Value = "ID объекта";
                worksheet.Cell("C2").Style.Font.SetBold().Font.FontSize = 12;

                worksheet.Cell("D2").Value = "Описание изменения";
                worksheet.Cell("D2").Style.Font.SetBold().Font.FontSize = 12;

                int row = 3;
                int column = 1;
                foreach (var jou in journals)
                {
                    worksheet.Cell(row, column).Value = $"{jou.Usercapture.surname} {jou.Usercapture.name} {jou.Usercapture.patronymic}";
                    worksheet.Cell(row, column + 1).Value = $"{jou.datetimechange.ToString("D")}";
                    worksheet.Cell(row, column + 2).Value = $"{jou.idobject}";
                    worksheet.Cell(row, column + 3).Value = $"{jou.description}";
                    row += 1;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{DateTime.Now}.xlsx");
                }
            }
        }
    }
}
