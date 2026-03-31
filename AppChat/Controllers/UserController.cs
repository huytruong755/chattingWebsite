using AppChat.Models;
using AppChat.Models.DTOs;
using AppChat.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AppChat.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;
        public UserController(UserService service) => _service = service;

        [HttpPost("new")]
        public async Task<IActionResult> CreateUser([FromBody] User newUser)
        {
            try
            {
                var user = await _service.CreateUser(newUser);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (Exception e) { return BadRequest(e.Message); }
        }

        // Get User by ID (full user info - admin only)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                return Ok(await _service.GetUserById(id));
            }
            catch (Exception e) { return BadRequest(e.Message); }
        }

        // Get User Profile (mini profile without password - for chat display)
        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetUserProfile(int id)
        {
            try
            {
                var profile = await _service.GetUserProfileAsync(id);
                if (profile == null) return NotFound("User not found");
                return Ok(profile);
            }
            catch (Exception e) { return BadRequest(e.Message); }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllUser()
        {
            try { return Ok(await _service.GetAllUsersAsync()); }
            catch (Exception e) { return BadRequest(e.Message); }
        }

        // Search users by name or phone
        [HttpGet("search/{term}")]
        public async Task<IActionResult> SearchUsers(string term)
        {
            try
            {
                var results = await _service.SearchUsersAsync(term);
                return Ok(results);
            }
            catch (Exception e) { return BadRequest(e.Message); }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            try
            {
                await _service.UpdateUser(id, updatedUser);
                return NoContent();
            }
            catch (Exception e) { return BadRequest(e.Message); }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _service.DeleteUser(id);
                return NoContent();
            }
            catch (Exception e) { return BadRequest(e.Message); }
        }
    }
}
