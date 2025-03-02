using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TsmartTask.Data;
using TsmartTask.DTOs;
using TsmartTask.Model;

namespace TsmartTask.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _context.Products.Where(x => !x.IsDeleted).ToList();

            if (products == null || products.Count == 0)
            {
                return NotFound(new { message = "Ürün bulunmamaktadır." });
            }

            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound(new { message = "Bu id sahip ürün bulunmamaktadır." });
            }
            return Ok(product);
        }

        [HttpPost]
        public IActionResult AddProduct([FromBody] dtoProductAdd productDto)
        {
            if (productDto == null)
            {
                return BadRequest(new { message = "Geçersiz ürün verisi." });
            }

            var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                Stock = productDto.Stock,
                IsDeleted = false 
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] dtoProductAdd updatedProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Ürün bulunamadı.");
            }
            if (!string.IsNullOrEmpty(updatedProduct.Name))
            {
                product.Name = updatedProduct.Name;
            }
            if (updatedProduct.Price != 0)
            {
                product.Price = updatedProduct.Price;
            }
            if (updatedProduct.Stock != 0)
            {
                product.Stock = updatedProduct.Stock;
            }
            try
            {
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(product);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Güncellemede hata oluştu.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var Product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (Product == null || Product.IsDeleted)
            {
                return NotFound(new { message = "Silinmek istenen ürün bulunamadı veya silinmiş durumda" });
            }

            Product.IsDeleted = true;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ürün silinirken bir hata oluştu.", error = ex.Message });
            }

            return Ok(new { message = "Ürün başarıyla silindi." });
        }

        [HttpGet("deleted")]
        public IActionResult Get()
        {
            var deletedProducts = _context.Products.Where(p => p.IsDeleted).ToList();
            if (!deletedProducts.Any())
            {
                return NotFound(new { message = "Silinmiş ürün bulunmamaktadır." });
            }

            return Ok(deletedProducts);
        }

        [HttpPut("{id}")]
        public IActionResult PatchRestore(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null || !product.IsDeleted)
            {
                return NotFound(new { message = "Geri getirilmek istenen ürün bulunamadı." });
            }
            product.IsDeleted = false;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ürün geri getirilirken bir hata oluştu.", error = ex.Message });
            }

            return Ok(new { message = "Ürün başarıyla geri getirildi." });
        }


    }
}
