using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private static readonly List<dynamic> Products = new()
        {
            new { Id = 1, Name = "Notebook Dell Inspiron", Category = "Informática", Price = 5500.00, Stock = 12, CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new { Id = 2, Name = "Teclado Mecânico Corsair", Category = "Periféricos", Price = 420.00, Stock = 35, CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new { Id = 3, Name = "Cadeira Gamer ThunderX3", Category = "Móveis", Price = 1600.00, Stock = 8, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new { Id = 4, Name = "Mouse Logitech G Pro", Category = "Periféricos", Price = 280.00, Stock = 60, CreatedAt = DateTime.UtcNow.AddDays(-1) }
        };

        [HttpPost]
        [Authorize(Roles = "SimpleUser")]
        public IActionResult Create([FromBody] dynamic newProduct)
        {
            return Ok(new
            {
                message = "Produto cadastrado com sucesso (mock).",
                data = newProduct
            });
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Manager")]
        public IActionResult GetById(int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound(new { message = "Produto não encontrado." });

            return Ok(new
            {
                message = $"Detalhes do produto {id}",
                data = product
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            return Ok(new
            {
                message = $"Produto {id} excluído com sucesso (mock)."
            });
        }
    }
}
