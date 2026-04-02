using AppChat.Models.DTOs;
using AppChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppChat.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class BlockingController : ControllerBase
    {
        private readonly BlockingService _blockingService;
        private readonly ILogger<BlockingController> _logger;

        public BlockingController(BlockingService blockingService, ILogger<BlockingController> logger)
        {
            _blockingService = blockingService;
            _logger = logger;
        }

        // POST: /blocking/block
        [HttpPost("block")]
        public async Task<IActionResult> BlockUser([FromBody] BlockUserRequest req)
        {
            try
            {
                // Get current user from JWT token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int blockerId))
                    return Unauthorized(new { message = "Invalid user ID in token" });

                // Prevent blocking yourself
                if (blockerId == req.BlockedUserId)
                    return BadRequest(new { message = "Không thể chặn chính mình" });

                var blockedUser = await _blockingService.BlockUserAsync(blockerId, req.BlockedUserId);
                return Ok(new { message = "Chặn người dùng thành công", data = blockedUser });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi chặn người dùng", error = ex.Message });
            }
        }

        // DELETE: /blocking/unblock
        [HttpDelete("unblock")]
        public async Task<IActionResult> UnblockUser([FromBody] BlockUserRequest req)
        {
            try
            {
                // Get current user from JWT token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int blockerId))
                    return Unauthorized(new { message = "Invalid user ID in token" });

                bool unblocked = await _blockingService.UnblockUserAsync(blockerId, req.BlockedUserId);
                if (!unblocked)
                    return NotFound(new { message = "Không tìm thấy người dùng bị chặn" });

                return Ok(new { message = "Bỏ chặn người dùng thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi bỏ chặn người dùng", error = ex.Message });
            }
        }

        // GET: /blocking/list
        [HttpGet("list")]
        public async Task<IActionResult> GetBlockedUsers()
        {
            try
            {
                // Get current user from JWT token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized(new { message = "Invalid user ID in token" });

                var blockedUsers = await _blockingService.GetBlockedUsersByIdAsync(userId);
                return Ok(blockedUsers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi lấy danh sách chặn", error = ex.Message });
            }
        }

        // GET: /blocking/check?blockedUserId=2
        [HttpGet("check")]
        public async Task<IActionResult> CheckIfBlocked([FromQuery] int blockedUserId)
        {
            try
            {
                // Get current user from JWT token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int blockerId))
                    return Unauthorized(new { message = "Invalid user ID in token" });

                // Check if current user blocked the other user
                bool isBlocked = await _blockingService.IsUserBlockedAsync(blockerId, blockedUserId);
                return Ok(new { isBlocked, message = isBlocked ? "Bạn đã chặn người dùng này" : "Bạn chưa chặn người dùng này" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi kiểm tra chặn", error = ex.Message });
            }
        }

        // GET: /blocking/check-both?userId2=2
        [HttpGet("check-both")]
        public async Task<IActionResult> CheckIfBlockedBothWays([FromQuery] int userId2)
        {
            try
            {
                // Get current user from JWT token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId1))
                    return Unauthorized(new { message = "Invalid user ID in token" });

                bool isBlocked = await _blockingService.IsBlockedBothWaysAsync(userId1, userId2);
                return Ok(new { 
                    isBlockedEitherWay = isBlocked, 
                    message = isBlocked ? "Bạn hoặc họ đã chặn nhau" : "Không ai chặn ai hết" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi kiểm tra chặn", error = ex.Message });
            }
        }

        public class BlockUserRequest
        {
            public int BlockedUserId { get; set; }
        }
    }
}
