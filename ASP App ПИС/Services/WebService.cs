using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Helpers;
using System.Text.Json;
using System.Text;
using System.Net;

namespace ASP_App_ПИС.Services
{
    public class WebService : IWebService
    {
        private readonly HttpClient _client;

        public WebService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<MunicipalityName>> GetMunicipalities()
        {
            var response = await _client.GetAsync("/api/Municipality");
            return await response.ReadContentAsync<List<MunicipalityName>>();
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

        public async Task<IEnumerable<ContractNumber>> GetContracts()
        {
            var response = await _client.GetAsync("/api/Contract");
            return await response.ReadContentAsync<List<ContractNumber>>();
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

        public async Task<IEnumerable<ActCapture>> GetAct(int locid)
        {
            var response = await _client.GetAsync($"/api/ActCapture/{locid}");
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

        /*public async Task<IEnumerable<TaskMonth>> GetTaskMonthsFromLocalityId(int id)
        {
            var response = await _client.GetAsync($"/api/TaskMonth/{id}");
            return await response.ReadContentAsync<IEnumerable<TaskMonth>>();
        }*/

        public async Task<IEnumerable<TaskMonth>> GetTaskMonth(int id)
        {
            var response = await _client.GetAsync($"/api/TaskMonth/{id}");
            return await response.ReadContentAsync<IEnumerable<TaskMonth>>();
        }

        public async Task<IEnumerable<ActCapture>> GetActs(DateTime datestart, DateTime dateend, int locid)
        {
            var response = await _client.GetAsync($"/api/ActCapture/{datestart}/{dateend}/{locid}");
            return await response.ReadContentAsync<IEnumerable<ActCapture>>();
        }

        public async Task<HttpResponseMessage> AddAct(ActCapture value)
        {
            string jsonString = JsonSerializer.Serialize(value);
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

        public async Task<HttpResponseMessage> AddMunicipality(MunicipalityName value)
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
            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"/api/TaskMonth/add", content);
        }

        public async Task<HttpResponseMessage> EditTaskMonth(int id, TaskMonth value)
        {
            string jsonString = JsonSerializer.Serialize(value);
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
    }
}
