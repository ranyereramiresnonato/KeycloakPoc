using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchasesApi.Services;

namespace PurchasesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private readonly IPaymentVoucherService _voucherService;
        public PurchasesController(IPaymentVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        private static readonly List<dynamic> Purchases = new()
        {
            new { Id = 1, User = "alice", Item = "Notebook Dell", Value = 5500.00, Date = DateTime.UtcNow.AddDays(-10) },
            new { Id = 2, User = "bob", Item = "Teclado Mecânico", Value = 420.00, Date = DateTime.UtcNow.AddDays(-5) },
            new { Id = 3, User = "carol", Item = "Cadeira Gamer", Value = 1600.00, Date = DateTime.UtcNow.AddDays(-2) },
            new { Id = 4, User = "bob", Item = "Mouse Logitech", Value = 280.00, Date = DateTime.UtcNow.AddDays(-1) }
        };


        [HttpPost]
        [Authorize(Roles = "SimpleUser")]
        public IActionResult Create([FromBody] dynamic newPurchase)
        {
            return Ok(new
            {
                message = "Compra registrada com sucesso (mock).",
                data = newPurchase
            });
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Manager")]
        public IActionResult GetById(int id)
        {
            var purchase = Purchases.FirstOrDefault(p => p.Id == id);
            if (purchase == null)
                return NotFound(new { message = "Compra não encontrada." });

            return Ok(new
            {
                message = $"Detalhes da compra {id}",
                data = purchase
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            return Ok(new
            {
                message = $"Compra {id} excluída com sucesso (mock)."
            });
        }

        [HttpGet("{id:int}/voucher")]
        [Authorize(Roles = "SimpleUser")]
        public async Task<IActionResult> GetVoucher(int id)
        {
            var purchase = Purchases.FirstOrDefault(p => p.Id == id);
            if (purchase == null)
                return NotFound(new { message = "Compra não encontrada." });

            var pdfBase64 = await _voucherService.GetVoucherBase64ByPurchaseIdAsync(id);

            if (string.IsNullOrEmpty(pdfBase64))
                return BadRequest(new { message = "Não foi possível obter o comprovante de pagamento." });

            return Ok(pdfBase64);
        }
    }
}
