namespace PaymentVoucherApi.Services
{
    public interface ITokenService
    {
        Task<string?> GetTokenAsync(string username, string password);
    }
}
