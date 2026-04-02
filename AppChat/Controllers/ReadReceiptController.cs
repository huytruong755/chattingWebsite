using AppChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppChat.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ReadReceiptController : ControllerBase
    {
        private readonly ReadReceiptService _readReceiptService;
        private readonly ILogger<ReadReceiptController> _logger;

        public ReadReceiptController(ReadReceiptService readReceiptService, ILogger<ReadReceiptController> logger)
        {
            _readReceiptService = readReceiptService;
            _logger = logger;
        }

        // POST: /readreceipt/mark
        [HttpPost("mark")]
        public async Task<IActionResult> MarkMessageAsRead([FromBody] MarkAsReadRequest req)
        {
            try
            {
                var readReceipt = await _readReceiptService.MarkMessageAsReadAsync(req.MessageId, req.UserId);
                return Ok(new { message = "Đã đánh dấu tin nhắn là đã đọc", data = readReceipt });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi đánh dấu tin nhắn", error = ex.Message });
            }
        }

        // GET: /readreceipt/message/{messageId}
        [HttpGet("message/{messageId}")]
        public async Task<IActionResult> GetReadReceipts(int messageId)
        {
            try
            {
                var receipts = await _readReceiptService.GetReadReceiptsByMessageIdAsync(messageId);
                return Ok(receipts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi lấy read receipts", error = ex.Message });
            }
        }

        // GET: /readreceipt/check?messageId=1&userId=2
        [HttpGet("check")]
        public async Task<IActionResult> CheckIfRead([FromQuery] int messageId, [FromQuery] int userId)
        {
            try
            {
                bool isRead = await _readReceiptService.IsMessageReadByUserAsync(messageId, userId);
                return Ok(new { isRead });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi kiểm tra read", error = ex.Message });
            }
        }

        // GET: /readreceipt/count/{messageId}
        [HttpGet("count/{messageId}")]
        public async Task<IActionResult> GetReadCount(int messageId)
        {
            try
            {
                int readCount = await _readReceiptService.GetReadCountAsync(messageId);
                return Ok(new { readCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi đếm read", error = ex.Message });
            }
        }

        public class MarkAsReadRequest
        {
            public int MessageId { get; set; }
            public int UserId { get; set; }
        }
    }
}
