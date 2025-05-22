using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApplication10.Data;
using WebApplication10.Models;

namespace WebApplication10.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(ILogger<AuthController> logger, IConfiguration config, ApplicationDbContext context)
    {
        _logger = logger;
        _config = config;
        _context = context;
    }

    [HttpPost("Giris-yap")]
    public async Task<IActionResult> SignIn([FromBody] Auth auth)
    {
        if (auth == null || string.IsNullOrEmpty(auth.UserName) || string.IsNullOrEmpty(auth.Password))
        {
            return BadRequest("Kullanıcı adı ve şifre gereklidir.");
        }

        // Kullanıcı adı ve şifre ile veritabanında kullanıcıyı bul
        var user = await _context.Users.FirstOrDefaultAsync(s =>
            s.UserName == auth.UserName && s.Password == auth.Password);

        if (user == null)
        {
            _logger.LogWarning("Geçersiz kullanıcı adı veya şifre denemesi: {UserName}", auth.UserName);
            return Unauthorized("Kullanıcı adı veya şifre yanlış.");
        }

        // JWT token oluştur
        var tokenString = GenerateJwtToken(user);
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadToken(tokenString) as JwtSecurityToken;

        // Token'ı sakla
        var storedToken = new StoredToken
        {
            Token = tokenString,
            Expiration = token.ValidTo,
            UserName = user.UserName,
            Password = user.Password,
            CreatedDate = DateTime.UtcNow
        };

        await _context.StoredTokens.AddAsync(storedToken);
        await _context.SaveChangesAsync();

        // Kullanıcı bilgileri ve token'i döndür
        return Ok(new
        {
            token = tokenString,
            userName = user.UserName,
            email = user.Email,
            phoneNumber = user.PhoneNumber,
            name = user.Name,
            lastName = user.LastName
        });
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim("id", user.Id.ToString()), // Kullanıcı ID'sini ekle
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}