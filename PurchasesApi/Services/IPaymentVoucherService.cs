namespace PurchasesApi.Services
{
    public interface IPaymentVoucherService
    {
        Task<string?> GetVoucherBase64ByPurchaseIdAsync(int purchaseId);
    }
}
