using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication10.Data;

namespace WebApplication10.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public UserController(ILogger<UserController> logger, ApplicationDbContext context,
            IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _config = configuration;
        }

        // Kayıt olma işlemi
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Password alanı boş olamaz.");
            }

            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Email ve Şifre zorunludur.");
            }

            // Email kontrolü
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return Conflict("Bu email adresi zaten kullanılıyor.");
            }

            user.CreatedDate = DateTime.Now;
            user.UpdatedDate = DateTime.Now;
            user.IsMember = true; // Varsayılan olarak üye kabul edilir

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok("Kayıt başarıyla tamamlandı.");
        }
        [HttpGet("getUserByUsername")]
        public async Task<IActionResult> GetUserByUsername(string username, string phone)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username && u.PhoneNumber == phone);

            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            return Ok(new
            {
                user.Name,
                user.LastName,
                user.UserName,
                user.Gender,
                user.Birthdate,
                user.PhoneNumber,
                user.Email
            });
        }

    }
}