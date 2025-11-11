using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PurchasesApi.Services
{
    public class PaymentVoucherService : IPaymentVoucherService
    {
        private readonly HttpClient _httpClient;

        public PaymentVoucherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> GetVoucherBase64ByPurchaseIdAsync(int purchaseId)
        {
            var loginPayload = new
            {
                Username = "microservice",
                Password = "12345678"
            };

            var loginJson = new StringContent(JsonSerializer.Serialize(loginPayload), Encoding.UTF8, "application/json");
            var loginUrl = "http://paymentvoucherapi:8080/api/PaymentVoucher/Login";

            var loginResponse = await _httpClient.PostAsync(loginUrl, loginJson);
            if (!loginResponse.IsSuccessStatusCode)
                return null;

            var loginContent = await loginResponse.Content.ReadAsStringAsync();
            using var loginDoc = JsonDocument.Parse(loginContent);

            if (!loginDoc.RootElement.TryGetProperty("access_token", out var accessTokenElement))
                return null;

            var token = accessTokenElement.GetString();
            if (string.IsNullOrEmpty(token))
                return null;

            var voucherUrl = $"http://paymentvoucherapi:8080/api/PaymentVoucher/Get-By-Purchase-Id/{purchaseId}";
            var request = new HttpRequestMessage(HttpMethod.Get, voucherUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var voucherResponse = await _httpClient.SendAsync(request);
            if (!voucherResponse.IsSuccessStatusCode)
                return null;

            var voucherContent = await voucherResponse.Content.ReadAsStringAsync();
            return voucherContent;
        }
    }
}
