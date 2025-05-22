using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using WebApplication10.DTOS;
using WebApplication10.Service;

namespace WebApplication10.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }
        
        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] CreateAddressDto addressDto)
        {
            if (addressDto == null || string.IsNullOrWhiteSpace(addressDto.AddressLine1))
            {
                return BadRequest("Adres bilgileri eksik.");
            }

            try
            {
                // Token'ı body'den alın
                var token = addressDto.Token;
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("Token eksik.");
                }

                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

                if (!handler.CanReadToken(token))
                {
                    return Unauthorized("Geçersiz token formatı.");
                }

                // Token'ı çözümle
                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "id");

                if (userIdClaim == null)
                {
                    return Unauthorized("Token içinde 'id' bulunamadı.");
                }

                int userId = int.Parse(userIdClaim.Value); // Kullanıcı ID'sini int'e çevir

                // Adres ekleme işlemi
                var addedAddress = await _addressService.AddAddressAsync(addressDto, userId);

                return CreatedAtAction(nameof(GetAddressById), new { id = addedAddress.Id }, addedAddress); // Adresi döndür
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = $"Bir hata oluştu: {ex.Message}" }); // Hata mesajı
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAddressesByUser()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                {
                    return Unauthorized("Kullanıcı kimliği bulunamadı. Token içinde 'id' eksik.");
                }

                int userId = int.Parse(userIdClaim.Value);
                var addresses = await _addressService.GetAddressesByUserIdAsync(userId);
                return Ok(addresses);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            try
            {
                var address = await _addressService.GetAddressByIdAsync(id);
                if (address == null)
                {
                    return NotFound("Adres bulunamadı.");
                }
                return Ok(address);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] CreateAddressDto addressDto)
        {
            if (addressDto == null || string.IsNullOrWhiteSpace(addressDto.AddressLine1))
            {
                return BadRequest("Adres bilgileri eksik.");
            }

            try
            {
                var updatedAddress = await _addressService.UpdateAddressAsync(id, addressDto);
                return Ok(updatedAddress);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                await _addressService.DeleteAddressAsync(id);
                return Ok(new { message = "Adres başarıyla silindi." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = $"Bir hata oluştu: {ex.Message}" });
            }
        }
    }
}
