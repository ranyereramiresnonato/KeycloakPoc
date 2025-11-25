namespace PurchasesApi.Services
{
    public interface IKeycloakService
    {
        Task<string> GetAdminTokenAsync();
        Task<string> GetUserCompositeRolesAsync(string userId);
        Task<string> GetUserClientRolesAsync(string userId);
    }
}
