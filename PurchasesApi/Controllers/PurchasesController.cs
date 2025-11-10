using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PurchasesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok(new { message = "Acesso autorizado! Token válido do Keycloak." });
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Administrator")]
        public IActionResult GetAdmin()
        {
            return Ok(new { message = "Acesso liberado para Administrator!" });
        }
    }
}
