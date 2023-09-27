using Domain;

namespace ASP_App_ПИС.Services.Interfaces
{
    public interface IWebService
    {
        Task<IEnumerable<Municipality>> GetMunicipalities();
    }
}
