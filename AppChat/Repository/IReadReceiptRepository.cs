using AppChat.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppChat.Repositories
{
    public interface IReadReceiptRepository
    {
        Task<ReadReceiptDto> MarkMessageAsReadAsync(int messageId, int userId);

        Task<List<ReadReceiptDto>> GetReadReceiptsByMessageIdAsync(int messageId);

        Task<bool> IsMessageReadByUserAsync(int messageId, int userId);

        Task<int> GetReadCountAsync(int messageId);
    }
}
