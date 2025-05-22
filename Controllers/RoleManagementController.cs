using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebApplication10.Models;

namespace WebApplication10.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RoleManagementController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleManagementController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // Rol yönetim sayfası
        [HttpGet("Index")]
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        // Kullanıcıya rol atama
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest("User not found");

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
                return BadRequest("Role not found");

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
                return Ok("Role assigned successfully");
            else
                return BadRequest("Role assignment failed");
        }

        // Yeni rol oluşturma
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return BadRequest("Role name is required");

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                    return Ok("Role created successfully");
                else
                    return BadRequest("Role creation failed");
            }

            return BadRequest("Role already exists");
        }
    }
}
