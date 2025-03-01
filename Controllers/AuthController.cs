using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TsmartTask.Data;
using TsmartTask.DTOs;
using TsmartTask.Model;
using TsmartTask.Services;

namespace TsmartTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;
        public AuthController(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] dtoLoginRequest request)
        {
            var user = new User() { Id = 3, Username = "", Password = "" };

            if (user == null)
            {
                return Unauthorized(new { message = "Geçersiz kullanıcı adı veya şifre." });
            }

            var token = _jwtService.GenerateToken(user);
            return Ok(new { token });
        }
    }
}
