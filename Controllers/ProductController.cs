using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                Price = productDto.Price.Value,
                Stock = productDto.Stock.Value,
                IsDeleted = false 
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Dictionary<string, object> productUpdates)
        {
            if (productUpdates == null || !productUpdates.Any())
            {
                return BadRequest(new
                {
                    message = "Lütfen geçerli bir JSON formatında veri gönderin.",
                    example = new
                    {
                        Name = "Yeni Ürün Adı",
                        Price = 150.75,
                        Stock = 100
                    },
                    details = "Lütfen Content-Type başlığını 'application/json' olarak ayarladığınızdan emin olun."
                });
            }

            // Geçerli alan adlarını tanımla
            var validFields = new[] { "Name", "Price", "Stock" };

            // Yanlış alan adlarını kontrol et
            var invalidFields = productUpdates.Keys.Where(k => !validFields.Contains(k)).ToList();
            if (invalidFields.Any())
            {
                return BadRequest(new
                {
                    message = "Geçersiz alan adları gönderildi.",
                    invalidFields,
                    validFields
                });
            }

            // Ürün mevcut mu kontrol et
            var existingProduct = _context.Products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound(new { message = "Güncellenmek istenen ürün bulunamadı." });
            }

            // Gelen veriler varsa, sadece bu alanları güncelle
            if (productUpdates.ContainsKey("Name") && productUpdates["Name"] != null)
                existingProduct.Name = productUpdates["Name"].ToString();

            if (productUpdates.ContainsKey("Price") && productUpdates["Price"] != null)
                existingProduct.Price = Convert.ToDecimal(productUpdates["Price"]);

            if (productUpdates.ContainsKey("Stock") && productUpdates["Stock"] != null)
                existingProduct.Stock = Convert.ToInt32(productUpdates["Stock"]);

            try
            {
                // Veritabanında güncelle
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ürün güncellenirken bir hata oluştu.", error = ex.Message });
            }

            return Ok(new { message = "Ürün başarıyla güncellendi." });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            // Ürün mevcut mu kontrol et
            var Product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (Product == null)
            {
                return NotFound(new { message = "Silinmek istenen ürün bulunamadı." });
            }

            Product.IsDeleted = true;

            try
            {
                // Veritabanında güncelle
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
            return Ok(deletedProducts);
        }

        [HttpPut("{id}")]
        public IActionResult PatchRestore(int id)
        {
            // Ürün mevcut mu kontrol et
            var existingProduct = _context.Products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound(new { message = "Geri getirilmek istenen ürün bulunamadı." });
            }

            // Soft delete'i geri al
            existingProduct.IsDeleted = false;

            try
            {
                // Veritabanında güncelle
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
