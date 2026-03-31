using AppChat.Data;
using AppChat.Models;
using AppChat.Models.DTOs;
using AppChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CSharpLearning.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly TokenService _tokenService;
        private readonly AppDbContext _context;

        public AuthController(TokenService tokenService, AppDbContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            try
            {
                // Check existing user
                var existingUser = await _context.Users.AnyAsync(u => u.PhoneNumber == dto.PhoneNumber);
                if (existingUser) return Ok(new { message = "User already exists" });

                // Create new user
                var newUser = new User
                {
                    PhoneNumber = dto.PhoneNumber,
                    Password = dto.Password,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };

                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
                return Created(string.Empty, new { message = "User created" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            try
            {
                // Check user
                var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);

                // Check if user information not valid
                if (user == null || user.Password != dto.Password)
                {
                    return Ok(new { message = "Invalid information" });
                }

                // Create and response token back to client
                var accessToken = _tokenService.GenerateToken(user.Id.ToString(), user.PhoneNumber);

                return Ok(new { accessToken, user.Id });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Lấy userId từ token
                var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

                if (!int.TryParse(userIdClaim, out int userId))
                    return BadRequest("Invalid user ID in token.");

                // Cập nhật trạng thái online = false
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.IsOnline = false;
                    user.LastSeen = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
