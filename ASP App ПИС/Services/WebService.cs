using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Helpers;
using System.Text.Json;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ASP_App_ПИС.Services
{
    public class WebService : IWebService
    {
        private readonly HttpClient _client;

        public WebService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<Municipality>> GetMunicipalities()
        {
            var response = await _client.GetAsync("/api/Municipality");
            return await response.ReadContentAsync<List<Municipality>>();
        }

        public async Task<IEnumerable<Locality>> GetLocalActs()
        {
            var response = await _client.GetAsync("/api/ActCapture");
            return await response.ReadContentAsync<List<Locality>>();
        }

        public async Task<IEnumerable<Animal>> GetAnimals(int id)
        {
            var response = await _client.GetAsync($"/api/Animal/{id}");
            return await response.ReadContentAsync<List<Animal>>();
        }

        public async Task<IEnumerable<Contract>> GetContracts()
        {
            var response = await _client.GetAsync("/api/Contract");
            return await response.ReadContentAsync<List<Contract>>();
        }

        public async Task<IEnumerable<Locality>> GetLocalities()
        {
            var response = await _client.GetAsync("/api/Locality");
            return await response.ReadContentAsync<List<Locality>>();
        }

        public async Task<IEnumerable<Schedule>> GetSchedules()
        {
            var response = await _client.GetAsync("/api/Schedule");
            return await response.ReadContentAsync<List<Schedule>>();
        }

        public async Task<IEnumerable<TaskMonth>> GetTaskMonths()
        {
            var response = await _client.GetAsync("/api/TaskMonth");
            return await response.ReadContentAsync<List<TaskMonth>>();
        }

        public async Task<IEnumerable<Usercapture>> GetUsers()
        {
            var response = await _client.GetAsync("/api/User");
            return await response.ReadContentAsync<List<Usercapture>>();
        }

        public async Task<Schedule> GetScheduleFromTaskMonthId(int id)
        {
            var response = await _client.GetAsync($"/api/Schedule/task/{id}");
            return await response.ReadContentAsync<Schedule>();
        }

        public async Task<Municipality> GetMunicipalityFromLocalityId(int id)
        {
            var response = await _client.GetAsync($"/api/Municipality/loc/{id}");
            return await response.ReadContentAsync<Municipality>();
        }

        public async Task<ActCapture> GetActFromAnimalId(int id)
        {
            var response = await _client.GetAsync($"/api/ActCapture/animal/{id}");
            return await response.ReadContentAsync<ActCapture>();
        }

        public async Task<IEnumerable<ActCapture>> GetAct(int locid)
        {
            var response = await _client.GetAsync($"/api/ActCapture/{locid}");
            return await response.ReadContentAsync<IEnumerable<ActCapture>>();
        }

        public async Task<IEnumerable<ActCapture>> GetActsFromConLocId(int conlocid)
        {
            var response = await _client.GetAsync($"/api/ActCapture/all/{conlocid}");
            return await response.ReadContentAsync<IEnumerable<ActCapture>>();
        }

        public async Task<ActCapture> GetOneAct(int id)
        {
            var response = await _client.GetAsync($"/api/ActCapture/one/{id}");
            return await response.ReadContentAsync<ActCapture>();
        }

        public async Task<Animal> GetAnimal(int id)
        {
            var response = await _client.GetAsync($"/api/Animal/{id}");
            return await response.ReadContentAsync<Animal>();
        }

        public async Task<Locality> GetLocality(int id)
        {
            var response = await _client.GetAsync($"/api/Locality/{id}");
            return await response.ReadContentAsync<Locality>();
        }

        public async Task<Locality> GetOneLocality(int id)
        {
            var response = await _client.GetAsync($"/api/Locality/one/{id}");
            return await response.ReadContentAsync<Locality>();
        }

        public async Task<IEnumerable<TaskMonth>> GetTaskMonth(int id)
        {
            var response = await _client.GetAsync($"/api/TaskMonth/{id}");
            return await response.ReadContentAsync<IEnumerable<TaskMonth>>();
        }

        public async Task<TaskMonth> GetTaskMonthOne(int id)
        {
            var response = await _client.GetAsync($"/api/TaskMonth/one/{id}");
            return await response.ReadContentAsync<TaskMonth>();
        }

        public async Task<Contract> GetContractOne(int id)
        {
            var response = await _client.GetAsync($"/api/Contract/one/{id}");
            return await response.ReadContentAsync<Contract>();
        }

        public async Task<Animal> GetAnimalOne(int id)
        {
            var response = await _client.GetAsync($"/api/Animal/one/{id}");
            return await response.ReadContentAsync<Animal>();
        }

        public async Task<IEnumerable<ActCapture>> GetActs(DateTime datestart, DateTime dateend, int locid)
        {
            var response = await _client.GetAsync($"/api/ActCapture/{datestart}/{dateend}/{locid}");
            return await response.ReadContentAsync<IEnumerable<ActCapture>>();
        }

        public async Task<HttpResponseMessage> AddAct(ActCapture value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            jsonString = jsonString.Replace("T00:00:00", "T00:00:00.0Z");
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/api/ActCapture/add", content);
        }

        public async Task<HttpResponseMessage> DeleteAct(int id)
        {
            return await _client.DeleteAsync($"/api/ActCapture/delete/{id}");
        }

        public async Task<HttpResponseMessage> EditAct(int id, ActCapture value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            jsonString = jsonString.Replace("T00:00:00", "T00:00:00.0Z");
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PutAsync($"/api/ActCapture/put/{id}", content);
        }

        public async Task<HttpResponseMessage> AddAnimal(Animal value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/api/Animal/add", content);
        }

        public async Task<HttpResponseMessage> EditAnimal(int id, Animal value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PutAsync($"/api/Animal/put/{id}", content);
        }

        public async Task<HttpResponseMessage> DeleteAnimal(int id)
        {
            return await _client.DeleteAsync($"/api/Animal/delete/{id}");
        }

        public async Task<HttpResponseMessage> AddContract(Contract value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            jsonString = jsonString.Replace("T00:00:00", "T00:00:00.0Z");
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/api/Contract/add", content);
        }

        public async Task<HttpResponseMessage> DeleteContract(int id)
        {
            return await _client.DeleteAsync($"/api/Contract/delete/{id}");
        }

        public async Task<HttpResponseMessage> EditContract(int id, Contract value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            jsonString = jsonString.Replace("T00:00:00", "T00:00:00.0Z");
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PutAsync($"/api/Contract/put/{id}", content);
        }

        public async Task<HttpResponseMessage> AddLocality(Locality value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/api/Locality/add", content);
        }

        public async Task<HttpResponseMessage> EditLocality(int id, Locality value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PutAsync($"/api/Locality/put/{id}", content);
        }

        public async Task<HttpResponseMessage> DeleteLocality(int id)
        {
            return await _client.DeleteAsync($"/api/Locality/delete/{id}");
        }

        public async Task<HttpResponseMessage> AddMunicipality(Municipality value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/api/Municipality/add", content);
        }

        public async Task<HttpResponseMessage> EditMunicipality(int id, Municipality value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PutAsync($"/api/Municipality/put/{id}", content);
        }

        public async Task<HttpResponseMessage> DeleteMunicipality(int id)
        {
            return await _client.DeleteAsync($"/api/Municipality/delete/{id}");
        }

        public async Task<HttpResponseMessage> AddSchedule(Schedule value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            jsonString = jsonString.Replace("T00:00:00", "T00:00:00.0Z");
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/api/Schedule/add", content);
        }

        public async Task<HttpResponseMessage> DeleteSchedule(int id)
        {
            return await _client.DeleteAsync($"/api/Schedule/delete/{id}");
        }

        public async Task<HttpResponseMessage> AddTaskMonth(TaskMonth value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            jsonString = jsonString.Replace("T00:00:00", "T00:00:00.0Z");
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/api/TaskMonth/add", content);
        }

        public async Task<HttpResponseMessage> EditTaskMonth(int id, TaskMonth value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            jsonString = jsonString.Replace("T00:00:00", "T00:00:00.0Z");
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PutAsync($"/api/TaskMonth/put/{id}", content);
        }

        public async Task<HttpResponseMessage> DeleteTaskMonth(int id)
        {
            return await _client.DeleteAsync($"/api/TaskMonth/delete/{id}");
        }

        public async Task<IEnumerable<Locality>> GetLocalitiesFromMunId(int id)
        {
            var response = await _client.GetAsync($"/api/Locality/{id}");
            return await response.ReadContentAsync<List<Locality>>();
        }

        public async Task<TaskMonth> GetLastTaskMonth()
        {
            var response = await _client.GetAsync($"/api/TaskMonth/last");
            return await response.ReadContentAsync<TaskMonth>();
        }

        public async Task<Schedule> GetLastSchedule(int locid)
        {
            var response = await _client.GetAsync($"/api/Schedule/last/{locid}");
            return await response.ReadContentAsync<Schedule>();
        }

        public async Task<Animal> GetLastAnimal()
        {
            var response = await _client.GetAsync($"/api/Animal/last/");
            return await response.ReadContentAsync<Animal>();
        }

        public async Task<Locality> GetLastLocality()
        {
            var response = await _client.GetAsync($"/api/Locality/last/");
            return await response.ReadContentAsync<Locality>();
        }

        public async Task<HttpResponseMessage> AddUser(Usercapture value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/api/User/add", content);
        }

        //public async Task<HttpResponseMessage> AddMunLoc(Municipality_Locality value)
        //{
        //    string jsonString = JsonSerializer.Serialize(value);
        //    HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
        //    return await _client.PostAsync($"/api/Municipality/add-loc", content);
        //}

        public async Task<double> GetReportsMoney(int conid)
        {
            var response = await _client.GetAsync($"/api/Reports/money/{conid}");
            return await response.ReadContentAsync<double>();
        }

        public async Task<Dictionary<int, int>> GetReportsSchedule(int conid, int locid)
        {
            var response = await _client.GetAsync($"/api/Reports/schedule/{conid}/{locid}");
            return await response.ReadContentAsync<Dictionary<int, int>>();
        }
        
        public async Task<FileStreamResult> GetExcelMoney(string startdate, string enddate, int munid, double d)
        {
            var stream = await _client.GetStreamAsync($"/api/Reports/money/export/{startdate}/{enddate}/{munid}/{d}");
            FileStreamResult file = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            file.FileDownloadName = "Отчет о выполненной работе за контракт.xlsx";
            return file;
        }

        public async Task<FileStreamResult> GetExcelSchedule(string startdate, string enddate, int munid, int locid, int plan, int fact)
        {
            var stream = await _client.GetStreamAsync($"/api/Reports/money/export/{startdate}/{enddate}/{munid}/{locid}/{plan}/{fact}");
            FileStreamResult file = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            file.FileDownloadName = "Отчет о выполненной работе по планам-графикам.xlsx";
            return file;
        }

        public async Task<int> GetContractFromMuniciaplity(int id)
        {
            var response = await _client.GetAsync($"/api/Contract/{id}");
            return await response.ReadContentAsync<int>();
        }

        public async Task<HttpResponseMessage> AddContractLocality(Contract_Locality value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/api/Contract_Locality/add", content);
        }

        public async Task<IEnumerable<Contract_Locality>> GetTariphForLocality(int id)
        {
            var response = await _client.GetAsync($"/api/Contract_Locality/{id}");
            return await response.ReadContentAsync<IEnumerable<Contract_Locality>>();
        }

        public async Task<Municipality> GetMunicipalityForId(int id)
        {
            var response = await _client.GetAsync($"/api/Municipality/{id}");
            return await response.ReadContentAsync<Municipality>();
        }

        public async Task<Contract_Locality> GetOneContract_Locality(int id)
        {
            var response = await _client.GetAsync($"/api/Contract_Locality/one/{id}");
            return await response.ReadContentAsync<Contract_Locality>();
        }

        public async Task<Contract_Locality> GetDateContract_LocalityForDate(int id, string datecapture)
        {
            var response = await _client.GetAsync($"/api/Contract_Locality/date/{id}/{datecapture}");
            return await response.ReadContentAsync<Contract_Locality>();
        }

        public async Task<HttpResponseMessage> EditTariphLocality(int id, Contract_Locality value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PutAsync($"/api/Contract_Locality/put/{id}", content);
        }

        public async Task<IEnumerable<Organization>> GetOrganizations()
        {
            var response = await _client.GetAsync("/api/Organization");
            return await response.ReadContentAsync<IEnumerable<Organization>>();
        }

        public async Task<HttpResponseMessage> AddOrganization(Organization value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/api/Organization/add", content);
        }

        public async Task<Organization> GetOneOrganization(int id)
        {
            var response = await _client.GetAsync($"/api/Organization/one/{id}");
            return await response.ReadContentAsync<Organization>();
        }

        public async Task<HttpResponseMessage> EditOrganization(int id, Organization value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PutAsync($"/api/Organization/put/{id}", content);
        }

        public async Task<HttpResponseMessage> DeleteOrganization(int id)
        {
            return await _client.DeleteAsync($"/api/Organization/delete/{id}");
        }

        public async Task<IEnumerable<Journal>> GetJournal(int id)
        {
            var response = await _client.GetAsync($"/api/Journal/{id}");
            return await response.ReadContentAsync<IEnumerable<Journal>>();
        }

        public async Task<HttpResponseMessage> AddJournal(Journal value)
        {
            string jsonString = JsonSerializer.Serialize(value);

            int indexT = jsonString.IndexOf('T');
            jsonString = jsonString.Remove(indexT + 9, 14);
            jsonString = jsonString.Insert(indexT + 9, ".0Z");

            /*var firstNum = jsonString[indexT + 1];
            var secongNum = jsonString[indexT + 2];
            var timeaa = $"{firstNum}{secongNum}";
            int hours = int.Parse(timeaa); // это беру часы от времени
            int nowHours = hours - 5;
            jsonString = jsonString.Remove(indexT + 1, 2); // удаляю текущее время
            jsonString = jsonString.Insert(indexT + 1, nowHours.ToString()); // добавляю нужное*/

            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/api/Journal/add", content);
        }

        public async Task<HttpResponseMessage> DeleteJournal(int id)
        {
            return await _client.DeleteAsync($"/api/Journal/delete/{id}");
        }

        public async Task<Contract> GetLastContract()
        {
            var response = await _client.GetAsync($"/api/Contract/last");
            return await response.ReadContentAsync<Contract>();
        }

        public async Task<Municipality> GetLastMunicipality()
        {
            var response = await _client.GetAsync($"/api/Municipality/last");
            return await response.ReadContentAsync<Municipality>();
        }

        public async Task<ActCapture> GetLastActCapture()
        {
            var response = await _client.GetAsync($"/api/ActCapture/last");
            return await response.ReadContentAsync<ActCapture>();
        }

        public async Task<Organization> GetLastOrganization()
        {
            var response = await _client.GetAsync($"/api/Organization/last");
            return await response.ReadContentAsync<Organization>();
        }

        public async Task<Usercapture> GetLastUser()
        {
            var response = await _client.GetAsync($"/api/User/last");
            return await response.ReadContentAsync<Usercapture>();
        }

        public async Task<IEnumerable<Role>> GetRoles()
        {
            var response = await _client.GetAsync("/api/Role");
            return await response.ReadContentAsync<List<Role>>();
        }

        public async Task<Role> GetOneRole(int id)
        {
            var response = await _client.GetAsync($"/api/Role/{id}");
            return await response.ReadContentAsync<Role>();
        }

        public async Task<Role> GetLastRole()
        {
            var response = await _client.GetAsync($"/api/Role/last");
            return await response.ReadContentAsync<Role>();
        }

        public async Task<HttpResponseMessage> AddRole(Role value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/api/Role/add", content);
        }

        public async Task<HttpResponseMessage> EditRole(int id, Role value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PutAsync($"/api/Role/put/{id}", content);
        }

        public async Task<HttpResponseMessage> DeleteRole(int id)
        {
            return await _client.DeleteAsync($"/api/Role/delete/{id}");
        }

        public async Task<IEnumerable<Contract_Locality>> GetContract_Localities()
        {
            var response = await _client.GetAsync("/api/Contract_Locality");
            return await response.ReadContentAsync<List<Contract_Locality>>();
        }

        public async Task<IEnumerable<Contract_Locality>> GetContract_LocalityFromConId(int id)
        {
            var response = await _client.GetAsync($"/api/Contract_Locality/{id}");
            return await response.ReadContentAsync<List<Contract_Locality>>();
        }

        public async Task<IEnumerable<Contract>> GetContractsFromMunId(int id)
        {
            var response = await _client.GetAsync($"/api/Contract/all/{id}");
            return await response.ReadContentAsync<List<Contract>>();
        }

        public async Task<Schedule> GetOneScheduleFromLocDate(int conlocid)
        {
            var response = await _client.GetAsync($"/api/Schedule/one/{conlocid}");
            return await response.ReadContentAsync<Schedule>();
        }

        public async Task<FileStreamResult> GetExcelJournal(int id)
        {
            var stream = await _client.GetStreamAsync($"/api/Journal/export/{id}");
            FileStreamResult file = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            file.FileDownloadName = "Журнал изменений.xlsx";
            return file;
        }

        public async Task<Contract_Locality> GetOneContract_LocalityFromId(int id)
        {
            var response = await _client.GetAsync($"/api/Contract_Locality/one/id/{id}");
            return await response.ReadContentAsync<Contract_Locality>();
        }

        public async Task<IEnumerable<Organization>> GetOneOrganizationFromLocId(int id)
        {
            var response = await _client.GetAsync($"/api/Organization/one/loc/{id}");
            return await response.ReadContentAsync<IEnumerable<Organization>>();
        }

        public async Task<IEnumerable<Animal>> GetActsCapture()
        {
            var response = await _client.GetAsync($"/api/ActCapture/alll");
            return await response.ReadContentAsync<IEnumerable<Animal>>();
        }

        public async Task<IEnumerable<Report>> GetRegisterMoney()
        {
            var response = await _client.GetAsync($"/api/Reports/register/money");
            return await response.ReadContentAsync<IEnumerable<Report>>();
        }

        public async Task<Report> GetOneRegisterMoney(int id)
        {
            var response = await _client.GetAsync($"/api/Reports/register/money/{id}");
            return await response.ReadContentAsync<Report>();
        }

        public async Task<HttpResponseMessage> AddReport(Report value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            jsonString = jsonString.Replace("T00:00:00", "T00:00:00.0Z");
            int indexT = jsonString.IndexOf('T', 170);
            jsonString = jsonString.Remove(indexT + 9, 14);
            jsonString = jsonString.Insert(indexT + 9, ".0Z");
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/api/Reports/add", content);
        }

        public async Task<HttpResponseMessage> EditReport(int id, Report value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            jsonString = jsonString.Replace("T00:00:00", "T00:00:00.0Z");
            int indexT = jsonString.IndexOf('T', 170);
            jsonString = jsonString.Remove(indexT + 9, 14);
            jsonString = jsonString.Insert(indexT + 9, ".0Z");
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PutAsync($"/api/Reports/put/{id}", content);
        }
    }
}
