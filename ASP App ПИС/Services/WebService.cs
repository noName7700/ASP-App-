using ASP_App_ПИС.Services.Interfaces;
using Domain;
using ASP_App_ПИС.Helpers;

namespace ASP_App_ПИС.Services
{
    public class WebService : IWebService
    {
        private readonly HttpClient _client;
        public const string testPath = "/api/Municipality";

        public WebService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<Municipality>> GetMunicipalities()
        {
            var response = await _client.GetAsync(testPath);
            return await response.ReadContentAsync<List<Municipality>>();
        }
    }
}
