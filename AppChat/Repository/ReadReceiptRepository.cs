using AppChat.Data;
using AppChat.Models;
using AppChat.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChat.Repositories
{
    public class ReadReceiptRepository : IReadReceiptRepository
    {
        private readonly AppDbContext _context;

        public ReadReceiptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ReadReceiptDto> MarkMessageAsReadAsync(int messageId, int userId)
        {
            var existingReceipt = await _context.ReadReceipts
                .FirstOrDefaultAsync(rr => rr.MessageId == messageId && rr.UserId == userId);

            if (existingReceipt != null)
                return MapToDto(existingReceipt);

            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
                throw new Exception("Message not found");

            var readReceipt = new ReadReceipt
            {
                MessageId = messageId,
                UserId = userId,
                ReadAt = DateTime.UtcNow
            };

            _context.ReadReceipts.Add(readReceipt);

            // Update message status if all recipients have read it
            message.Status = "read";
            await _context.SaveChangesAsync();

            return MapToDto(readReceipt);
        }

        public async Task<List<ReadReceiptDto>> GetReadReceiptsByMessageIdAsync(int messageId)
        {
            var receipts = await _context.ReadReceipts
                .Where(rr => rr.MessageId == messageId)
                .ToListAsync();

            return receipts.Select(MapToDto).ToList();
        }

        public async Task<bool> IsMessageReadByUserAsync(int messageId, int userId)
        {
            return await _context.ReadReceipts
                .AnyAsync(rr => rr.MessageId == messageId && rr.UserId == userId);
        }

        public async Task<int> GetReadCountAsync(int messageId)
        {
            return await _context.ReadReceipts
                .Where(rr => rr.MessageId == messageId)
                .CountAsync();
        }

        private ReadReceiptDto MapToDto(ReadReceipt receipt)
        {
            return new ReadReceiptDto
            {
                Id = receipt.Id,
                MessageId = receipt.MessageId,
                UserId = receipt.UserId,
                ReadAt = receipt.ReadAt
            };
        }
    }
}
