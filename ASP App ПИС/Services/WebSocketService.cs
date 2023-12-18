using ASP_App_ПИС.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace ASP_App_ПИС.Services
{
    public class WebSocketService : IWebService
    {
        private ClientWebSocket _webSocket;
        public WebSocketService(HttpClient webSocket)
        {
            _webSocket = new ClientWebSocket();
        }

        public async Task<IEnumerable<Municipality>> GetMunicipalities()
        {
            await _webSocket.ConnectAsync(new Uri("https://localhost:7022/muns"), CancellationToken.None);
            ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
            WebSocketReceiveResult result = await _webSocket.ReceiveAsync(bytesReceived, CancellationToken.None);
            var response = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);
            Console.WriteLine(response);
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> AddAct(ActCapture value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> AddAnimal(Animal value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> AddContract(Contract value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> AddContractLocality(Contract_Locality value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> AddJournal(Journal value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> AddLocality(Locality value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> AddMunicipality(Municipality value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> AddOrganization(Organization value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> AddReport(Report value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> AddRole(Role value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> AddSchedule(Schedule value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> AddTaskMonth(TaskMonth value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> AddUser(Usercapture value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> DeleteAct(int id)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> DeleteAnimal(int id)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> DeleteContract(int id)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> DeleteJournal(int id)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> DeleteLocality(int id)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> DeleteMunicipality(int id)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> DeleteOrganization(int id)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> DeleteRole(int id)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> DeleteSchedule(int id)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> DeleteTaskMonth(int id)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> EditAct(int id, ActCapture value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> EditAnimal(int id, Animal value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> EditContract(int id, Contract value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> EditLocality(int id, Locality value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> EditMunicipality(int id, Municipality value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> EditOrganization(int id, Organization value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> EditReport(int id, Report value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> EditRole(int id, Role value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> EditTariphLocality(int id, Contract_Locality value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> EditTaskMonth(int id, TaskMonth value)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> EditUser(int id, Usercapture value)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ActCapture>> GetAct(int locid)
        {
            throw new NotImplementedException();
        }

        public Task<ActCapture> GetActFromAnimalId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Animal>> GetActsCapture()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ActCapture>> GetActsFromConLocId(int conlocid)
        {
            throw new NotImplementedException();
        }

        public Task<Animal> GetAnimal(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Animal> GetAnimalOne(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Animal>> GetAnimals(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetContractFromMuniciaplity(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Contract> GetContractOne(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Contract>> GetContracts()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Contract>> GetContractsFromMunId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Contract_Locality>> GetContract_Localities()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Contract_Locality>> GetContract_LocalityFromConId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Contract_Locality> GetDateContract_LocalityForDate(int id, string datecapture)
        {
            throw new NotImplementedException();
        }

        public Task<FileStreamResult> GetExcelJournal(int id)
        {
            throw new NotImplementedException();
        }

        public Task<FileStreamResult> GetExcelMoney(string startdate, string enddate, int munid, double d)
        {
            throw new NotImplementedException();
        }

        public Task<FileStreamResult> GetExcelSchedule(string startdate, string enddate, int munid, int locid, int plan, int fact)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Journal>> GetJournal(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ActCapture> GetLastActCapture()
        {
            throw new NotImplementedException();
        }

        public Task<Animal> GetLastAnimal()
        {
            throw new NotImplementedException();
        }

        public Task<Contract> GetLastContract()
        {
            throw new NotImplementedException();
        }

        public Task<Locality> GetLastLocality()
        {
            throw new NotImplementedException();
        }

        public Task<Municipality> GetLastMunicipality()
        {
            throw new NotImplementedException();
        }

        public Task<Organization> GetLastOrganization()
        {
            throw new NotImplementedException();
        }

        public Task<Report> GetLastReport()
        {
            throw new NotImplementedException();
        }

        public Task<Role> GetLastRole()
        {
            throw new NotImplementedException();
        }

        public Task<Schedule> GetLastSchedule(int locid)
        {
            throw new NotImplementedException();
        }

        public Task<TaskMonth> GetLastTaskMonth()
        {
            throw new NotImplementedException();
        }

        public Task<Usercapture> GetLastUser()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Locality>> GetLocalActs()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Locality>> GetLocalities()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Locality>> GetLocalitiesFromMunId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Locality> GetLocality(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Municipality> GetMunicipalityForId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Municipality> GetMunicipalityFromLocalityId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ActCapture> GetOneAct(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Contract_Locality> GetOneContract_Locality(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Contract_Locality> GetOneContract_LocalityFromId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Locality> GetOneLocality(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Organization> GetOneOrganization(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Organization>> GetOneOrganizationFromLocId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Report> GetOneRegisterMoney(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Role> GetOneRole(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Schedule> GetOneScheduleFromLocDate(int conlocid)
        {
            throw new NotImplementedException();
        }

        public Task<Status> GetOneStatus(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Usercapture> GetOneUser(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Organization>> GetOrganizations()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Report>> GetRegisterMoney()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Report>> GetRegisterSchedule()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Report>> GetReportFromMun(int id)
        {
            throw new NotImplementedException();
        }

        public Task<double> GetReportsMoney(int conid)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<int, int>> GetReportsSchedule(int conid, int locid)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Role>> GetRoles()
        {
            throw new NotImplementedException();
        }

        public Task<Schedule> GetScheduleFromTaskMonthId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Schedule>> GetSchedules()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Status>> GetStatusesReports()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Contract_Locality>> GetTariphForLocality(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TaskMonth>> GetTaskMonth(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TaskMonth> GetTaskMonthOne(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TaskMonth>> GetTaskMonths()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Usercapture>> GetUsers()
        {
            throw new NotImplementedException();
        }
    }
}
