using System.Net.Http.Headers;
using System.Text.Json;

namespace PurchasesApi.Services
{

    public class KeycloakService : IKeycloakService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public KeycloakService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> GetAdminTokenAsync()
        {
            string baseUrl = _config["KeycloakValidation:BaseUrl"];
            string realm = _config["KeycloakValidation:Realm"];
            string clientId = _config["KeycloakValidation:ClientId"];
            string username = _config["KeycloakValidation:Username"];
            string password = _config["KeycloakValidation:Password"];

            string url = $"{baseUrl}/realms/{realm}/protocol/openid-connect/token";

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", clientId },
            { "username", username },
            { "password", password }
        });

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao obter token: {response.StatusCode}");

            string responseString = await response.Content.ReadAsStringAsync();

            var json = JsonDocument.Parse(responseString);
            return json.RootElement.GetProperty("access_token").GetString();
        }

        public async Task<string> GetUserCompositeRolesAsync(string userId)
        {
            string baseUrl = _config["KeycloakValidation:BaseUrl"];
            string realm = _config["KeycloakValidation:Realm"];

            string token = await GetAdminTokenAsync();

            string url =
                $"{baseUrl}/admin/realms/{realm}/users/{userId}/role-mappings/realm/composite";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao buscar roles: {response.StatusCode}");

            return await response.Content.ReadAsStringAsync();
        }
    }
}
