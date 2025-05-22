using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication10.Data;

namespace WebApplication10.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tüm menüyü listele
        /// </summary>
        [HttpGet]
        public IActionResult GetMenu()
        {
            var menu = _context.Products.ToList();
            if (!menu.Any())
            {
                return NotFound("Menüde ürün bulunamadı.");
            }
            return Ok(menu);
        }

        /// <summary>
        /// Belirli bir kategorideki ürünleri listele
        /// </summary>
        [HttpGet("category/{category}")]
        public IActionResult GetMenuByCategory(string category)
        {
            var products = _context.Products
                                   .Where(p => p.Category.ToLower() == category.ToLower())
                                   .ToList();

            if (!products.Any())
            {
                return NotFound($"'{category}' kategorisinde ürün bulunamadı.");
            }
            return Ok(products);
        }

        /// <summary>
        /// Belirli bir ürünün detaylarını getir
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound($"ID {id} olan ürün bulunamadı.");
            }
            return Ok(product);
        }

        /// <summary>
        /// Yeni ürün ekle (Yönetici için)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Ürün bilgileri eksik.");
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        /// <summary>
        /// Belirli bir ürünü güncelle (Yönetici için)
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound($"ID {id} olan ürün bulunamadı.");
            }

            product.Name = updatedProduct.Name;
            product.Category = updatedProduct.Category;
            product.Price = updatedProduct.Price;
            product.UpdatedAt = System.DateTime.UtcNow;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        /// <summary>
        /// Belirli bir ürünü sil (Yönetici için)
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound($"ID {id} olan ürün bulunamadı.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok($"ID {id} olan ürün silindi.");
        }
    }
}
