using Domain;

namespace ASP_App_ПИС.Services.Interfaces
{
    public interface IWebService
    {
        Task<IEnumerable<MunicipalityName>> GetMunicipalities();
        Task<IEnumerable<Locality>> GetLocalActs();
        Task<IEnumerable<Animal>> GetAnimals(int id, string date);
        Task<IEnumerable<ContractNumber>> GetContracts();
        Task<IEnumerable<Locality>> GetLocalities();
        Task<IEnumerable<Locality>> GetLocalitiesFromMunId(int id);
        Task<IEnumerable<Schedule>> GetSchedules();
        Task<IEnumerable<TaskMonth>> GetTaskMonths();
        Task<TaskMonth> GetLastTaskMonth();
        Task<Schedule> GetLastSchedule(int locid);
        Task<Animal> GetLastAnimal();
        Task<Locality> GetLastLocality();
        Task<IEnumerable<ActCapture>> GetAct(int locid);
        Task<Animal> GetAnimal(int id);
        /*Task<Locality> GetLocality(int id);*/
        Task<Locality> GetOneLocality(int id);
        Task<IEnumerable<TaskMonth>> GetTaskMonth(int id);
        Task<IEnumerable<ActCapture>> GetActs(DateTime datestart, DateTime dateend, int locid);
        Task<HttpResponseMessage> AddAct(ActCapture value);
        Task<HttpResponseMessage> DeleteAct(int id);
        Task<HttpResponseMessage> EditAct(int id, ActCapture value); // реализовать в server
        Task<HttpResponseMessage> AddAnimal(Animal value);
        Task<HttpResponseMessage> EditAnimal(int id, Animal value);
        Task<HttpResponseMessage> DeleteAnimal(int id);
        Task<HttpResponseMessage> AddContract(ContractNumber value);
        Task<HttpResponseMessage> DeleteContract(int id);
        Task<HttpResponseMessage> EditContract(int id, Contract value); // реализовать в server
        Task<HttpResponseMessage> AddLocality(Locality value);
        Task<HttpResponseMessage> EditLocality(int id, Locality value);
        Task<HttpResponseMessage> DeleteLocality(int id);
        Task<HttpResponseMessage> AddMunicipality(MunicipalityName value);
        Task<HttpResponseMessage> EditMunicipality(int id, MunicipalityName value);
        Task<HttpResponseMessage> DeleteMunicipality(int id);  // нам вообще нужно будет удалять муниципалитет ???
        Task<HttpResponseMessage> AddSchedule(Schedule value);
        Task<HttpResponseMessage> DeleteSchedule(int id);
        Task<HttpResponseMessage> AddTaskMonth(TaskMonth value);
        Task<HttpResponseMessage> EditTaskMonth(int id, TaskMonth value);
        Task<HttpResponseMessage> DeleteTaskMonth(int id);
        Task<HttpResponseMessage> AddMunLoc(Municipality_Locality value);

    }
}
