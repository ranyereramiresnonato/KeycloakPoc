using Microsoft.AspNetCore.Mvc;
using PaymentVoucherApi.DTO;
using PaymentVoucherApi.Services;
using System.Threading.Tasks;

namespace PaymentVoucherApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateToken([FromBody] LoginRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Usuário e senha são obrigatórios.");

            var token = await _tokenService.GetTokenAsync(request.Username, request.Password);

            if (token == null)
                return Unauthorized("Falha ao autenticar no Keycloak.");

            return Ok(new { access_token = token });
        }
    }
}
