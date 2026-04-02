using AppChat.Models;
using AppChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AppChat.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _service;

        public ChatController(ChatService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet] // https://localhost:5047/chat
        public async Task<IActionResult> GetConversations()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return BadRequest("Invalid user ID in token.");

                var conversations = await _service.GetConversationsAsync(userId);
                // Check if conversations are null?
                if (conversations == null || !conversations.Any())
                {
                    return Ok(new List<Chat>());
                }

                return Ok(conversations);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        // GET: /chat/{id}
        [Authorize]
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChatById(int chatId)
        {
            try
            {
                var chat = await _service.GetChatByIdAsync(chatId);
                if (chat == null)
                    return NotFound(new { message = "Chat không tồn tại" });

                return Ok(chat);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        // DELETE: /chat/{id}
        [Authorize]
        [HttpDelete("{chatId}")]
        public async Task<IActionResult> DeleteChat(int chatId)
        {
            try
            {
                bool deleted = await _service.DeleteChatAsync(chatId);
                if (!deleted)
                    return NotFound(new { message = "Chat không tồn tại" });

                return Ok(new { message = "Xóa chat thành công" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        // PUT: /chat/{id}/archive
        [Authorize]
        [HttpPut("{chatId}/archive")]
        public async Task<IActionResult> ArchiveChat(int chatId)
        {
            try
            {
                bool archived = await _service.ArchiveChatAsync(chatId);
                if (!archived)
                    return NotFound(new { message = "Chat không tồn tại" });

                return Ok(new { message = "Lưu trữ chat thành công" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        // PUT: /chat/{id}/unarchive
        [Authorize]
        [HttpPut("{chatId}/unarchive")]
        public async Task<IActionResult> UnarchiveChat(int chatId)
        {
            try
            {
                bool unarchived = await _service.UnarchiveChatAsync(chatId);
                if (!unarchived)
                    return NotFound(new { message = "Chat không tồn tại" });

                return Ok(new { message = "Bỏ lưu trữ chat thành công" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
