using System.Text.Json;

namespace PaymentVoucherApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public TokenService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string?> GetTokenAsync(string username, string password)
        {
            var keycloakSection = _config.GetSection("Keycloak");
            var clientId = keycloakSection["ClientId"];
            var clientSecret = keycloakSection["ClientSecret"];
            var loginUrl = keycloakSection["LoginUrl"];

            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            var response = await _httpClient.PostAsync(loginUrl, form);

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(content);

            return json.GetProperty("access_token").GetString();
        }
    }
}
