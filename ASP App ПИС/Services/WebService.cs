using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Helpers;
using System.Text.Json;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System;

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

        public async Task<IEnumerable<Animal>> GetAnimals(int id, string date)
        {
            var response = await _client.GetAsync($"/api/Animal/{id}/{date}");
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

        public async Task<IEnumerable<ActCapture>> GetActs(int locid, string date)
        {
            var response = await _client.GetAsync($"/api/ActCapture/{locid}/{date}");
            return await response.ReadContentAsync<IEnumerable<ActCapture>>();
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

        public async Task<double> GetReportsMoney(string startDate, string endDate, int munid)
        {
            var response = await _client.GetAsync($"/api/Reports/{startDate}/{endDate}/{munid}");
            return await response.ReadContentAsync<double>();
        }

        public async Task<Dictionary<int, int>> GetReportsSchedule(int munid)
        {
            var response = await _client.GetAsync($"/api/Reports/{munid}");
            return await response.ReadContentAsync<Dictionary<int, int>>();
        }

        
        public Task<FileStreamResult> GetExcelMoney()
        {
            throw new NotImplementedException();
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
            jsonString = jsonString.Replace("T00:00:00", "T00:00:00.0Z");

            //int indexT = jsonString.IndexOf('T'); // индекс Т к нему прибавляю 9
            //jsonString = jsonString.Remove(indexT + 9, 14); // удаляю там что-то
            //jsonString = jsonString.Insert(indexT + 9, ".0Z"); // добавляю это

            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/api/Journal/add", content);
        }

        public async Task<HttpResponseMessage> DeleteJournal(int id)
        {
            return await _client.DeleteAsync($"/api/Journal/delete/{id}");
        }
    }
}
