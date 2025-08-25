using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockService.Contracts;
using StockService.Domain;
using StockService.Infrastructure;

namespace StockService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly StockDbContext _db;
        public ProductsController(StockDbContext db) => _db = db;

        // POST /api/products
        [HttpPost]
        public async Task<ActionResult<ProductResponse>> Create([FromBody] CreateProductDto dto)
        {
            var p = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity
            };
            _db.Products.Add(p);
            await _db.SaveChangesAsync();

            var response = new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.Quantity);
            return CreatedAtAction(nameof(GetById), new { id = p.Id }, response);
        }

        // GET /api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll()
            => await _db.Products
                .Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.Quantity))
                .ToListAsync();

        // GET /api/products/{id}  (útil pra testes)
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductResponse>> GetById([FromRoute] int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p is null) return NotFound();
            return new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.Quantity);
        }

        // PUT /api/products/{id}/estoque  (atualização manual de estoque)
        [HttpPut("{id:int}/estoque")]
        public async Task<IActionResult> UpdateStock([FromRoute] int id, [FromBody] UpdateStockDto dto)
        {
            var p = await _db.Products.FindAsync(id);
            if (p is null) return NotFound();
            p.Quantity = dto.Quantity; // define valor absoluto
            await _db.SaveChangesAsync();
            return NoContent();
        }

    }
}
