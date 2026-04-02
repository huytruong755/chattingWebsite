using AppChat.Models.DTOs;
using AppChat.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppChat.Services
{
    public class ReadReceiptService
    {
        private readonly IReadReceiptRepository _repo;

        public ReadReceiptService(IReadReceiptRepository repo) => _repo = repo;

        public async Task<ReadReceiptDto> MarkMessageAsReadAsync(int messageId, int userId)
        {
            return await _repo.MarkMessageAsReadAsync(messageId, userId);
        }

        public async Task<List<ReadReceiptDto>> GetReadReceiptsByMessageIdAsync(int messageId)
        {
            return await _repo.GetReadReceiptsByMessageIdAsync(messageId);
        }

        public async Task<bool> IsMessageReadByUserAsync(int messageId, int userId)
        {
            return await _repo.IsMessageReadByUserAsync(messageId, userId);
        }

        public async Task<int> GetReadCountAsync(int messageId)
        {
            return await _repo.GetReadCountAsync(messageId);
        }
    }
}
