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
            var products = _context.Products.ToList();

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

    }
}
