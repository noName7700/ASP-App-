using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace ASP_App_ПИС.Services.Interfaces
{
    public interface IWebService
    {
        Task<IEnumerable<Municipality>> GetMunicipalities();
        Task<IEnumerable<Locality>> GetLocalActs();
        Task<IEnumerable<Animal>> GetAnimals(int id);
        Task<IEnumerable<Contract>> GetContracts();
        Task<IEnumerable<Locality>> GetLocalities();
        Task<IEnumerable<Locality>> GetLocalitiesFromMunId(int id);
        Task<IEnumerable<Schedule>> GetSchedules();
        Task<IEnumerable<Usercapture>> GetUsers();
        Task<Schedule> GetScheduleFromTaskMonthId(int id);
        Task<Municipality> GetMunicipalityFromLocalityId(int id);
        Task<ActCapture> GetActFromAnimalId(int id);
        Task<IEnumerable<TaskMonth>> GetTaskMonths();
        Task<TaskMonth> GetLastTaskMonth();
        Task<Municipality> GetLastMunicipality();
        Task<Schedule> GetLastSchedule(int locid);
        Task<Animal> GetLastAnimal();
        Task<Locality> GetLastLocality();
        Task<IEnumerable<ActCapture>> GetAct(int locid);
        Task<IEnumerable<ActCapture>> GetActsFromConLocId(int conlocid);
        Task<Animal> GetAnimal(int id);
        Task<Locality> GetLocality(int id);
        Task<Locality> GetOneLocality(int id);
        Task<IEnumerable<TaskMonth>> GetTaskMonth(int id, int conid);
        Task<TaskMonth> GetTaskMonthOne(int id);
        Task<Contract> GetContractOne(int id);
        Task<Animal> GetAnimalOne(int id);
        //Task<IEnumerable<ActCapture>> GetActs(DateTime datestart, DateTime dateend, int locid);
        Task<ActCapture> GetOneAct(int id);
        Task<HttpResponseMessage> AddAct(ActCapture value);
        Task<HttpResponseMessage> DeleteAct(int id);
        Task<HttpResponseMessage> EditAct(int id, ActCapture value);
        Task<HttpResponseMessage> AddAnimal(Animal value);
        Task<HttpResponseMessage> EditAnimal(int id, Animal value);
        Task<HttpResponseMessage> DeleteAnimal(int id);
        Task<HttpResponseMessage> AddContract(Contract value);
        Task<HttpResponseMessage> DeleteContract(int id);
        Task<HttpResponseMessage> EditContract(int id, Contract value);
        Task<Contract> GetLastContract();
        Task<HttpResponseMessage> AddLocality(Locality value);
        Task<HttpResponseMessage> EditLocality(int id, Locality value);
        Task<HttpResponseMessage> DeleteLocality(int id);
        Task<HttpResponseMessage> AddMunicipality(Municipality value);
        Task<HttpResponseMessage> EditMunicipality(int id, Municipality value);
        Task<HttpResponseMessage> DeleteMunicipality(int id);  // нам вообще нужно будет удалять муниципалитет ???
        Task<HttpResponseMessage> AddSchedule(Schedule value);
        Task<HttpResponseMessage> DeleteSchedule(int id);
        Task<HttpResponseMessage> AddTaskMonth(TaskMonth value);
        Task<HttpResponseMessage> EditTaskMonth(int id, TaskMonth value);
        Task<HttpResponseMessage> DeleteTaskMonth(int id);
        Task<HttpResponseMessage> AddUser(Usercapture value);
        //Task<HttpResponseMessage> AddMunLoc(Municipality_Locality value);

        // отчет - деньги
        Task<double> GetReportsMoney(int conid);
        Task<FileStreamResult> GetExcelMoney(string startdate, string enddate, int munid, double d);
        Task<FileStreamResult> GetExcelSchedule(string startdate, string enddate, int munid, int locid, int plan, int fact);
        Task<FileStreamResult> GetExcelJournal(int id);

        // отчет - план-график
        Task<Dictionary<int, int>> GetReportsSchedule(int conid, int locid);

        // метод найти контракт по муниципалитету
        Task<int> GetContractFromMuniciaplity(int id);

        Task<HttpResponseMessage> AddContractLocality(Contract_Locality value);
        Task<IEnumerable<Contract_Locality>> GetTariphForLocality(int id);

        Task<Municipality> GetMunicipalityForId(int id);

        Task<Contract_Locality> GetOneContract_Locality(int id);

        Task<HttpResponseMessage> EditTariphLocality(int id, Contract_Locality value);
        Task<IEnumerable<Organization>> GetOrganizations();
        Task<Organization> GetOneOrganization(int id);
        Task<HttpResponseMessage> AddOrganization(Organization value);
        Task<HttpResponseMessage> EditOrganization (int id, Organization value);
        Task<HttpResponseMessage> DeleteOrganization(int id);
        Task<IEnumerable<Journal>> GetJournal(int id);
        Task<HttpResponseMessage> AddJournal(Journal value);
        Task<HttpResponseMessage> DeleteJournal(int id);
        Task<ActCapture> GetLastActCapture();
        Task<Organization> GetLastOrganization();
        Task<Usercapture> GetLastUser();
        Task<IEnumerable<Role>> GetRoles();
        Task<Role> GetOneRole(int id);
        Task<Role> GetLastRole();
        Task<HttpResponseMessage> AddRole(Role value);
        Task<HttpResponseMessage> EditRole(int id, Role value);
        Task<HttpResponseMessage> DeleteRole(int id);
        Task<IEnumerable<Contract_Locality>> GetContract_Localities();
        Task<IEnumerable<Contract_Locality>> GetContract_LocalityFromConId(int id);
        Task<Contract_Locality> GetDateContract_LocalityForDate(int id, string datecapture);
        Task<IEnumerable<Contract>> GetContractsFromMunId(int id);
        Task<Schedule> GetOneScheduleFromLocDate(int locid, string startdate);
    }
}
